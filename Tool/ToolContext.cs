using System.Collections.Generic;
using Game.Common;
using Game.Input;
using Game.Tools;
using NetworkMultitool.Net;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace NetworkMultitool.Tool
{
    /// <summary>Per-frame state handed to the current mode. Owned by the system and refilled every frame.</summary>
    public sealed class ToolContext
    {
        private readonly NetworkMultitoolSystem _system;
        private readonly ToolOutputBarrier _barrier;
        private readonly Dictionary<ToolAction, ProxyAction> _actions;
        private EntityCommandBuffer _ecb;
        private bool _hasEcb;

        internal ToolContext(NetworkMultitoolSystem system, ToolOutputBarrier barrier, Dictionary<ToolAction, ProxyAction> actions)
        {
            _system = system;
            _barrier = barrier;
            _actions = actions;
        }

        public EntityManager Em { get; internal set; }

        public NetQuery Net { get; internal set; }

        public Settings Settings => Mod.Settings;

        /// <summary>Raycast owner under the mouse (node, edge or Entity.Null).</summary>
        public Entity Hover { get; internal set; }

        public RaycastHit Hit { get; internal set; }

        public bool HasHit { get; internal set; }

        public bool HoverIsNode { get; internal set; }

        public bool HoverIsEdge { get; internal set; }

        /// <summary>Raycast hit position on a node, an edge or the terrain; keeps the last value when nothing is hit.</summary>
        public float3 MousePosition { get; internal set; }

        /// <summary>Where the mouse ray crosses the horizontal plane at height <paramref name="y"/>: the ray runs from the camera through <see cref="MousePosition"/>,
        /// so a shape floating above the ground (elevated road) is grabbed where it is seen, not where the terrain under the cursor is.</summary>
        public float3 MouseAtHeight(float y)
        {
            var camera = UnityEngine.Camera.main;
            if (camera == null)
                return MousePosition;
            float3 eye = camera.transform.position;
            var dy = MousePosition.y - eye.y;
            // A ray nearly parallel to the plane, or a plane above the camera, has no usable crossing.
            if (math.abs(dy) < 1e-3f || (y - eye.y) / dy <= 0f)
                return MousePosition;
            return eye + (MousePosition - eye) * ((y - eye.y) / dy);
        }

        /// <summary>Mouse primary click this frame (selection). Validation is <c>Pressed(ToolAction.Apply)</c> (Enter key or UI Apply button); modes must use that one to validate.</summary>
        public bool ApplyPressed { get; internal set; }

        public bool ApplyHeld { get; internal set; }

        public bool SecondaryPressed { get; internal set; }

        /// <summary>Ctrl + left click this frame, outside the UI. Read from the mouse directly: the game blocks the Apply binding while a modifier it does not list is held.</summary>
        public bool PickPressed { get; internal set; }

        /// <summary>Set by the UI Apply button; consumed by the next <see cref="Pressed"/> of <see cref="ToolAction.Apply"/>.</summary>
        public bool ApplyRequested { get; set; }

        /// <summary>Reset to Clear before each Update: the previous Temp preview is replaced by the definitions emitted this frame.
        /// A mode validating sets Apply and emits nothing, so the Temp entities built from last frame's definitions become permanent.</summary>
        public ApplyMode ApplyMode { get; set; }

        public float MaxSegmentLength => Settings != null && Settings.SegmentLength > 0f ? Settings.SegmentLength : 80f;

        /// <summary>Shift 10, Ctrl 0.1, Alt 0.01, otherwise 1.</summary>
        public float StepMultiplier
        {
            get
            {
                var keyboard = Keyboard.current;
                if (keyboard == null)
                    return 1f;
                if (keyboard.shiftKey.isPressed)
                    return 10f;
                if (keyboard.ctrlKey.isPressed)
                    return 0.1f;
                if (keyboard.altKey.isPressed)
                    return 0.01f;
                return 1f;
            }
        }

        public bool CtrlHeld => Keyboard.current != null && Keyboard.current.ctrlKey.isPressed;

        /// <summary>Name of a prefab entity, "?" when unknown.</summary>
        public string PrefabName(Entity prefab) => _system.PrefabName(prefab);

        /// <summary>Command buffer of the tool output barrier, created on first use each frame.</summary>
        public EntityCommandBuffer Ecb
        {
            get
            {
                if (!_hasEcb)
                {
                    _ecb = _barrier.CreateCommandBuffer();
                    _hasEcb = true;
                }

                return _ecb;
            }
        }

        public bool Pressed(ToolAction action)
        {
            if (action == ToolAction.Apply && ApplyRequested)
            {
                ApplyRequested = false;
                return true;
            }

            return _actions.TryGetValue(action, out var proxy) && proxy.WasPressedThisFrame();
        }

        /// <summary>The game renders its own Temp preview of the emitted courses (Hide game preview is off).</summary>
        public bool GamePreviewShown => Settings == null || !Settings.HideGamePreview;

        public CourseEmitter Emitter() => new CourseEmitter(Ecb, Net) { HidePreview = !GamePreviewShown };

        /// <summary>
        /// Turns an edge hover into a node hover when the hit is close to one of the edge ends: plain nodes between two segments have almost no
        /// geometry of their own to raycast. The radius is capped at a third of the edge so its middle always stays an edge hover.
        /// </summary>
        public void SnapHoverToNode()
        {
            if (!HasHit || !HoverIsEdge || !Em.Exists(Hover))
                return;
            var ends = Net.Ends(Hover);
            var start = Net.NodePosition(ends.m_Start);
            var end = Net.NodePosition(ends.m_End);
            var radius = math.min(math.max(Net.Width(Hover) * 0.5f, 4f), math.distance(start, end) / 3f);
            var toStart = math.distance(MousePosition, start);
            var toEnd = math.distance(MousePosition, end);
            if (math.min(toStart, toEnd) > radius)
                return;
            Hover = toStart <= toEnd ? ends.m_Start : ends.m_End;
            HoverIsNode = true;
            HoverIsEdge = false;
        }

        /// <summary>Terrain height (water ignored) under a world position.</summary>
        public float TerrainHeight(float3 position) => _system.TerrainHeight(position);

        /// <summary>Drops the cached buffer once the barrier may have played it back, so the next use gets a fresh one.</summary>
        internal void ResetEcb() => _hasEcb = false;
    }
}
