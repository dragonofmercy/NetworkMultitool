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

        public bool ApplyPressed { get; internal set; }

        public bool ApplyHeld { get; internal set; }

        public bool SecondaryPressed { get; internal set; }

        /// <summary>What the game should do with the definitions emitted this frame; reset to None before each Update.</summary>
        public ApplyMode ApplyMode { get; set; }

        public float MaxSegmentLength => Settings?.SegmentLength ?? 80f;

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

        public bool Pressed(ToolAction action) => _actions.TryGetValue(action, out var proxy) && proxy.WasPressedThisFrame();

        public CourseEmitter Emitter(Entity prefab) => new CourseEmitter(Ecb, Net, prefab);

        public void SetMode(ToolModeId id) => _system.SetMode(id);

        /// <summary>Drops the cached buffer once the barrier may have played it back, so the next use gets a fresh one.</summary>
        internal void ResetEcb() => _hasEcb = false;
    }
}
