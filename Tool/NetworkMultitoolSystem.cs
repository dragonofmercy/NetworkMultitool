using System;
using System.Collections.Generic;
using Colossal.Collections;
using Colossal.Mathematics;
using Game;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Game.SceneFlow;
using Game.Tools;
using NetworkMultitool.Modes;
using NetworkMultitool.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace NetworkMultitool.Tool
{
    /// <summary>The single game tool; delegates per-frame work to the current <see cref="IToolMode"/>.</summary>
    public partial class NetworkMultitoolSystem : ToolBaseSystem
    {
        private readonly Dictionary<ToolModeId, IToolMode> m_Modes = new Dictionary<ToolModeId, IToolMode>();
        private readonly Dictionary<ToolAction, ProxyAction> m_Actions = new Dictionary<ToolAction, ProxyAction>();

        private const Layer Layers = Layer.Road | Layer.TrainTrack | Layer.TramTrack | Layer.SubwayTrack | Layer.Pathway;
        private const float NodeMarkerRange = 500f;

        private ToolOutputBarrier m_Barrier;
        private OverlayRenderSystem m_Overlay;
        private Game.Simulation.TerrainSystem m_Terrain;
        private EntityQuery m_DefinitionQuery;
        private Game.Net.SearchSystem m_NetSearch;
        private NetQuery m_Net;
        private ToolContext m_Context;
        private IToolMode m_Mode;
        private bool m_Running;
        private readonly List<(ProxyAction Proxy, Action<ProxyAction, InputActionPhase> Handler)> m_Bindings = new List<(ProxyAction, Action<ProxyAction, InputActionPhase>)>();

        public override string toolID => "NetworkMultitool";

        public ToolModeId Mode => m_Mode.Id;

        public bool IsActive => m_ToolSystem.activeTool == this;

        public string Info => m_Mode.Info;

        /// <summary>World labels drawn by the mode last frame, positioned on screen by the UI system.</summary>
        public List<(float3 Position, float3 Direction, string Text)> Labels { get; } = new List<(float3, float3, string)>();

        internal float TerrainHeight(float3 position)
        {
            var data = m_Terrain.GetHeightData();
            return Game.Simulation.TerrainUtils.SampleHeight(ref data, position);
        }

        internal string PrefabName(Entity prefab) => prefab != Entity.Null && m_PrefabSystem.TryGetPrefab(prefab, out PrefabBase value) && value != null ? value.name : "?";

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
            m_Barrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_Overlay = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            m_Terrain = World.GetOrCreateSystemManaged<Game.Simulation.TerrainSystem>();
            m_DefinitionQuery = GetDefinitionQuery();
            m_NetSearch = World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
            m_Net = new NetQuery(EntityManager);
            m_Context = new ToolContext(this, m_Barrier, m_Actions) { Em = EntityManager, Net = m_Net };

            RegisterMode(new IdleMode());
            RegisterMode(new AddNodeMode());
            RegisterMode(new RemoveNodeMode());
            RegisterMode(new SlopeMode());
            RegisterMode(new ArrangeLineMode());
            RegisterMode(new CurveMode());
            RegisterMode(new LoopMode());
            RegisterMode(new ConnectionMode());
            RegisterMode(new ParallelMode());
            m_Mode = m_Modes[ToolModeId.None];

            BindActions();
        }

        private void RegisterMode(IToolMode mode) => m_Modes[mode.Id] = mode;

        public override PrefabBase GetPrefab() => null;

        public override bool TrySetPrefab(PrefabBase prefab) => false;

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();
            m_ToolRaycastSystem.typeMask = TypeMask.Net | TypeMask.Terrain;
            m_ToolRaycastSystem.netLayerMask = Layers;
            m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground | CollisionMask.Underground;
            m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            m_Context.ResetEcb();
            applyAction.shouldBeEnabled = true;
            secondaryApplyAction.shouldBeEnabled = true;
            SetToolActionsEnabled(true);
            m_Running = true;
            m_Mode.Enter(m_Context);
        }

        // Also reached when the game switches to another tool.
        protected override void OnStopRunning()
        {
            m_Mode.Exit(m_Context);
            // Closing the panel forgets the mode: it reopens with no tool selected (a mode key still opens it on that mode, SetMode runs before activation).
            m_Mode = m_Modes[ToolModeId.None];
            m_Running = false;
            m_Context.ApplyRequested = false;
            SetToolActionsEnabled(false);
            base.OnStopRunning();
            m_Context.ResetEcb();
        }

        protected override void OnDestroy()
        {
            foreach (var (proxy, handler) in m_Bindings)
                proxy.onInteraction -= handler;
            m_Bindings.Clear();
            base.OnDestroy();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            try
            {
                return UpdateFrame(inputDeps);
            }
            finally
            {
                // The barrier plays the buffer back after this update; out-of-frame calls must not reuse it.
                m_Context.ResetEcb();
            }
        }

        private JobHandle UpdateFrame(JobHandle inputDeps)
        {
            // Definitions live one frame: the mode re-emits them every frame and the game rebuilds the Temp preview from them.
            inputDeps = DestroyDefinitions(m_DefinitionQuery, m_Barrier, inputDeps);
            var blocked = m_FocusChanged || (m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) != 0;

            // Over the UI the mode still runs (without mouse input) so its preview survives and a UI Apply click validates it like the Enter key.
            if (blocked)
                FillBlockedContext();
            else
                FillContext();
            var mode = m_Mode;
            Labels.Clear();
            try
            {
                mode.Update(m_Context);
                applyMode = m_Context.ApplyMode;
                var buffer = m_Overlay.GetBuffer(out var overlayDeps);
                overlayDeps.Complete();
                var overlay = new Overlay(buffer, m_Net, Labels);
                if (mode.Id != ToolModeId.None)
                    RenderNodeMarkers(overlay);
                mode.Render(m_Context, overlay);
                m_Overlay.AddBufferWriter(default);
            }
            catch (Exception e)
            {
                // Logged once: the failing mode is dropped instead of throwing every frame.
                Mod.Log.Error(e, $"{nameof(NetworkMultitoolSystem)}: mode {mode.Id} failed, switching to {ToolModeId.None}");
                applyMode = ApplyMode.Clear;
                // The failing mode is not exited: its Exit may throw too.
                m_Mode = m_Modes[ToolModeId.None];
                m_Mode.Enter(m_Context);
            }
            finally
            {
                // A request the mode did not consume this frame must not fire later in another mode.
                m_Context.ApplyRequested = false;
            }

            return inputDeps;
        }

        // Every node of the editable networks around the cursor, so the player sees where segments start and end in any mode.
        private void RenderNodeMarkers(Overlay overlay)
        {
            var mouse = m_Context.MousePosition.xz;
            var tree = m_NetSearch.GetNetSearchTree(true, out var dependencies);
            dependencies.Complete();
            var collector = new NetCollector { Bounds = new Bounds2(mouse - NodeMarkerRange, mouse + NodeMarkerRange), Hits = new NativeList<Entity>(Allocator.Temp) };
            tree.Iterate(ref collector);
            foreach (var entity in collector.Hits)
            {
                // The tree also holds edges, previews and the nets owned by buildings.
                if (!m_Net.IsNode(entity) || m_Net.HasOwner(entity) || EntityManager.HasComponent<Temp>(entity) || !m_Net.OnLayer(entity, Layers))
                    continue;
                var position = m_Net.NodePosition(entity);
                if (math.distancesq(position.xz, mouse) < NodeMarkerRange * NodeMarkerRange)
                    overlay.NodeMarker(entity, position);
            }

            collector.Hits.Dispose();
        }

        private struct NetCollector : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
        {
            public Bounds2 Bounds;
            public NativeList<Entity> Hits;

            public bool Intersect(QuadTreeBoundsXZ bounds) => MathUtils.Intersect(bounds.m_Bounds.xz, Bounds);

            public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
            {
                if (Intersect(bounds))
                    Hits.Add(entity);
            }
        }

        /// <summary>Activates the tool, or returns to the default tool when already active.</summary>
        public void Toggle()
        {
            if (!InGame)
                return;
            m_Context.ApplyRequested = false;
            if (IsActive)
                m_ToolSystem.activeTool = m_DefaultToolSystem;
            else
                Activate();
        }

        /// <summary>Switches to the mode and activates the tool. Safe to call between frames.</summary>
        public void SetMode(ToolModeId id)
        {
            if (!InGame)
                return;
            if (!m_Modes.TryGetValue(id, out var next))
            {
                Mod.Log.Warn($"{nameof(NetworkMultitoolSystem)}: mode {id} is not registered");
                return;
            }

            if (next != m_Mode)
            {
                // When not running, OnStartRunning enters the new mode.
                if (m_Running)
                    m_Mode.Exit(m_Context);
                m_Context.ApplyRequested = false;
                m_Mode = next;
                if (m_Running)
                    m_Mode.Enter(m_Context);
            }

            if (!IsActive)
                Activate();
        }

        /// <summary>Same as pressing the Apply key on the next tool update; ignored while the tool is inactive.</summary>
        public void RequestApply()
        {
            if (IsActive)
                m_Context.ApplyRequested = true;
        }

        // Bindings stay enabled in the main menu and editor; the tool only runs in a loaded game.
        private static bool InGame => GameManager.instance != null && GameManager.instance.gameMode.IsGame();

        private void Activate()
        {
            m_ToolSystem.selected = Entity.Null;
            m_ToolSystem.activeTool = this;
        }

        private void FillContext()
        {
            var hasHit = GetRaycastResult(out Entity entity, out RaycastHit hit);
            m_Context.HasHit = hasHit;
            m_Context.Hover = entity;
            m_Context.Hit = hit;
            m_Context.HoverIsNode = hasHit && m_Net.IsNode(entity);
            m_Context.HoverIsEdge = hasHit && m_Net.IsEdge(entity);
            if (hasHit)
                m_Context.MousePosition = hit.m_HitPosition;
            m_Context.ApplyPressed = applyAction.WasPressedThisFrame();
            m_Context.ApplyHeld = applyAction.IsPressed();
            m_Context.SecondaryPressed = secondaryApplyAction.WasPressedThisFrame();
            m_Context.PickPressed = m_Context.CtrlHeld && UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
            m_Context.ApplyMode = ApplyMode.Clear;
        }

        // Keeps the last world hover, hit and mouse position; no fresh hit and no mouse input.
        private void FillBlockedContext()
        {
            m_Context.HasHit = false;
            m_Context.ApplyPressed = false;
            m_Context.ApplyHeld = false;
            m_Context.SecondaryPressed = false;
            m_Context.PickPressed = false;
            m_Context.ApplyMode = ApplyMode.Clear;
        }

        private void SetToolActionsEnabled(bool enabled)
        {
            foreach (var action in m_Actions.Values)
                action.shouldBeEnabled = enabled;
        }

        // ToolAction names and "Mode" + ToolModeId names match the ToolActions constants.
        private void BindActions()
        {
            var settings = Mod.Settings;
            if (settings == null)
            {
                Mod.Log.Error($"{nameof(NetworkMultitoolSystem)}: settings not loaded, key bindings are disabled");
                return;
            }

            foreach (ToolAction action in Enum.GetValues(typeof(ToolAction)))
            {
                var proxy = FindAction(settings, action.ToString());
                if (proxy != null)
                    m_Actions[action] = proxy;
            }

            BindPermanent(settings, ToolActions.Toggle, Toggle);
            foreach (ToolModeId id in Enum.GetValues(typeof(ToolModeId)))
            {
                if (id != ToolModeId.None)
                    BindPermanent(settings, "Mode" + id, () => SetMode(id));
            }
        }

        private void BindPermanent(Settings settings, string name, Action onPerformed)
        {
            var proxy = FindAction(settings, name);
            if (proxy == null)
                return;
            proxy.shouldBeEnabled = true;
            Action<ProxyAction, InputActionPhase> handler = (_, phase) =>
            {
                if (phase != InputActionPhase.Performed)
                    return;
                try
                {
                    onPerformed();
                }
                catch (Exception e)
                {
                    Mod.Log.Error(e, $"{nameof(NetworkMultitoolSystem)}: key binding {name} failed");
                }
            };
            proxy.onInteraction += handler;
            m_Bindings.Add((proxy, handler));
        }

        private static ProxyAction FindAction(Settings settings, string name)
        {
            var proxy = settings.GetAction(name);
            if (proxy == null)
                Mod.Log.Error($"{nameof(NetworkMultitoolSystem)}: key binding {name} not found");
            return proxy;
        }
    }
}
