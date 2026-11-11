using System;
using System.Collections.Generic;
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
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.InputSystem;

namespace NetworkMultitool.Tool
{
    /// <summary>The single game tool; delegates per-frame work to the current <see cref="IToolMode"/>.</summary>
    public partial class NetworkMultitoolSystem : ToolBaseSystem
    {
        private readonly Dictionary<ToolModeId, IToolMode> m_Modes = new Dictionary<ToolModeId, IToolMode>();
        private readonly Dictionary<ToolAction, ProxyAction> m_Actions = new Dictionary<ToolAction, ProxyAction>();

        private ToolOutputBarrier m_Barrier;
        private OverlayRenderSystem m_Overlay;
        private EntityQuery m_DefinitionQuery;
        private NetQuery m_Net;
        private ToolContext m_Context;
        private IToolMode m_Mode;
        private bool m_Running;
        private readonly List<(ProxyAction Proxy, Action<ProxyAction, InputActionPhase> Handler)> m_Bindings = new List<(ProxyAction, Action<ProxyAction, InputActionPhase>)>();

        public override string toolID => "NetworkMultitool";

        public ToolModeId Mode => m_Mode.Id;

        public bool IsActive => m_ToolSystem.activeTool == this;

        public string Info => m_Mode.Info;

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
            m_Barrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_Overlay = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
            m_DefinitionQuery = GetDefinitionQuery();
            m_Net = new NetQuery(EntityManager);
            m_Context = new ToolContext(this, m_Barrier, m_Actions) { Em = EntityManager, Net = m_Net };

            // One line per mode; later modes register here.
            RegisterMode(new IdleMode());
            RegisterMode(new AddNodeMode());
            RegisterMode(new RemoveNodeMode());
            RegisterMode(new SlopeMode());
            RegisterMode(new ArrangeLineMode());
            RegisterMode(new ArrangeCircleMode());
            RegisterMode(new CurveMode());
            RegisterMode(new LoopMode());
            RegisterMode(new ConnectionMode());
            RegisterMode(new SplitNodeMode());
            m_Mode = m_Modes[ToolModeId.None];

            BindActions();
        }

        /// <summary>Adds or replaces the mode for its <see cref="IToolMode.Id"/>.</summary>
        public void RegisterMode(IToolMode mode) => m_Modes[mode.Id] = mode;

        public override PrefabBase GetPrefab() => null;

        public override bool TrySetPrefab(PrefabBase prefab) => false;

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();
            m_ToolRaycastSystem.typeMask = TypeMask.Net | TypeMask.Terrain;
            m_ToolRaycastSystem.netLayerMask = Layer.Road | Layer.TrainTrack | Layer.TramTrack | Layer.SubwayTrack | Layer.Pathway;
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
            try
            {
                mode.Update(m_Context);
                applyMode = m_Context.ApplyMode;
                var buffer = m_Overlay.GetBuffer(out var overlayDeps);
                overlayDeps.Complete();
                mode.Render(m_Context, new Overlay(buffer, m_Net));
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
            m_Context.ApplyMode = ApplyMode.Clear;
        }

        // Keeps the last world hover, hit and mouse position; no fresh hit and no mouse input.
        private void FillBlockedContext()
        {
            m_Context.HasHit = false;
            m_Context.ApplyPressed = false;
            m_Context.ApplyHeld = false;
            m_Context.SecondaryPressed = false;
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
