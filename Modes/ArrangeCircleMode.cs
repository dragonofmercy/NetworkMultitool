using System.Collections.Generic;
using System.Globalization;
using Game.Net;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace NetworkMultitool.Modes
{
    /// <summary>Puts the selected chain on a circle the user can move and resize before applying (port of CS1 ArrangeCircle).</summary>
    public sealed class ArrangeCircleMode : NodeChainMode
    {
        private const float MaxRadius = 5000f;
        private const float GrabDistance = 5f;
        private const float BigDelta = 50f;

        private static readonly Color Orange = new Color(1f, 0.5f, 0f, 0.6f);

        private enum Drag
        {
            None,
            Center,
            Radius,
        }

        private bool _editing;
        private bool _wrongShape;
        private Drag _drag;
        private float3 _center;
        private float _radius;
        private float _minRadius;
        private bool _evenly;
        private bool _clockwise;
        private bool _wrongOrder;
        private bool _bigDelta;
        private float3[] _positions;
        private float3[] _targets;

        public override ToolModeId Id => ToolModeId.ArrangeCircle;

        public override string Info
        {
            get
            {
                if (_wrongShape)
                    return "Wrong shape";
                if (!_editing)
                    return base.Info;

                return "Radius: " + _radius.ToString("0.0", CultureInfo.InvariantCulture) + " m\nUp/Down: change radius\nDrag center or circle edge\nCtrl+D: distribute evenly (" + (_evenly ? "on" : "off") + ")\nEnter: apply, right click: back";
            }
        }

        public override void Update(ToolContext ctx)
        {
            if (!_editing)
            {
                base.Update(ctx);
                return;
            }

            if (!IsValid(ctx))
            {
                Clear(ctx);
                return;
            }

            if (ctx.SecondaryPressed)
            {
                StopEditing();
                return;
            }

            if (ctx.Pressed(ToolAction.Apply))
            {
                Apply(ctx);
                Clear(ctx);
                return;
            }

            var changed = false;
            if (ctx.Pressed(ToolAction.RadiusUp))
            {
                _radius = ClampRadius(_radius + 10f * ctx.StepMultiplier);
                changed = true;
            }

            if (ctx.Pressed(ToolAction.RadiusDown))
            {
                _radius = ClampRadius(_radius - 10f * ctx.StepMultiplier);
                changed = true;
            }

            if (ctx.Pressed(ToolAction.Distribute))
            {
                _evenly = !_evenly;
                changed = true;
            }

            var distance = math.distance(Vec.Flat(ctx.MousePosition), Vec.Flat(_center));
            if (ctx.ApplyPressed && ctx.HasHit)
                _drag = distance < GrabDistance ? Drag.Center : math.abs(distance - _radius) < GrabDistance ? Drag.Radius : Drag.None;
            if (!ctx.ApplyHeld)
                _drag = Drag.None;

            // MousePosition is stale when nothing is hit: the circle then stays where it is.
            if (ctx.HasHit && _drag == Drag.Center)
            {
                _center = Vec.WithY(ctx.MousePosition, _center.y);
                changed = true;
            }
            else if (ctx.HasHit && _drag == Drag.Radius)
            {
                _radius = ClampRadius(distance);
                changed = true;
            }

            if (changed)
                UpdateTargets();
        }

        public override void Render(ToolContext ctx, Overlay overlay)
        {
            if (!_editing)
            {
                base.Render(ctx, overlay);
                return;
            }

            foreach (var node in Nodes)
                overlay.Node(node, Overlay.White);

            overlay.CircleOutline(_center, _radius, _wrongOrder ? Overlay.Red : _bigDelta ? Orange : Overlay.Green);
            overlay.Disc(_center, GrabDistance, Overlay.White);
            foreach (var target in _targets)
                overlay.Disc(target, _minRadius, Overlay.Yellow);
        }

        protected override void OnApplyRequested(ToolContext ctx)
        {
            var net = ctx.Net;
            _positions = new float3[Nodes.Count];
            for (var i = 0; i < Nodes.Count; i++)
                _positions[i] = net.NodePosition(Nodes[i]);

            if (!CircleArranger.TryFit(_positions, out _center, out _radius))
            {
                _wrongShape = true;
                return;
            }

            // Widest chain edge: lower radius bound and target disc size.
            _minRadius = 0f;
            for (var i = 0; i < Nodes.Count - 1; i++)
                _minRadius = math.max(_minRadius, net.Width(ChainEdge(ctx, i)));

            _wrongShape = false;
            _evenly = false;
            _drag = Drag.None;
            _editing = true;
            UpdateTargets();
        }

        protected override void OnChainChanged(ToolContext ctx)
        {
            _wrongShape = false;
            StopEditing();
        }

        protected override void Apply(ToolContext ctx)
        {
            var em = ctx.Em;
            var net = ctx.Net;
            var count = Nodes.Count;
            var points = new Point[count];
            var chainEdges = new HashSet<Entity>();
            for (var i = 0; i < count - 1; i++)
                chainEdges.Add(ChainEdge(ctx, i));

            for (var i = 0; i < count; i++)
            {
                var oldPosition = net.NodePosition(Nodes[i]);
                _targets[i] = Vec.WithY(_targets[i], oldPosition.y);
                var tangent = Vec.Turn90(Vec.NormalizeXZ(_targets[i] - _center), _clockwise);
                points[i] = new Point(_targets[i], tangent);
                // Edges outside the chain (at the ends) keep following the moved node; chain edges are rebuilt below.
                NodeMover.ShiftConnectedEdges(em, net, Nodes[i], _targets[i] - oldPosition, chainEdges);
                NodeMover.MoveNode(em, Nodes[i], _targets[i], NetUtils.GetNodeRotation(tangent));
            }

            RebuildChainEdges(ctx, points);

            foreach (var node in Nodes)
                NodeMover.MarkUpdated(em, node);
        }

        protected override void RenderPreview(ToolContext ctx, Overlay overlay)
        {
        }

        private void StopEditing()
        {
            _editing = false;
            _drag = Drag.None;
        }

        private float ClampRadius(float radius) => math.clamp(radius, _minRadius, MaxRadius);

        private void UpdateTargets()
        {
            _targets = CircleArranger.Distribute(_positions, _center, _radius, _evenly);
            _bigDelta = false;
            for (var i = 0; i < _targets.Length; i++)
                _bigDelta |= math.distance(_targets[i], _positions[i]) > BigDelta;

            // Rotational direction and order check (CS1 States): the chain must turn monotonically one way around the center.
            var angles = CircleArranger.Angles(_positions, _center);
            var sweep = 0f;
            var hasPositive = false;
            var hasNegative = false;
            for (var i = 1; i < angles.Length; i++)
            {
                var delta = angles[i] - angles[i - 1];
                delta -= 2f * math.PI * math.round(delta / (2f * math.PI));
                sweep += delta;
                hasPositive |= delta > 0f;
                hasNegative |= delta < 0f;
            }

            _clockwise = sweep < 0f;
            _wrongOrder = _clockwise ? hasPositive : hasNegative;
        }
    }
}
