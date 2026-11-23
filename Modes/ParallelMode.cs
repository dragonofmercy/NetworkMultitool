using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Tools;
using NetworkMultitool.Geometry;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;

namespace NetworkMultitool.Modes
{
    /// <summary>Builds a new network alongside the selected chain, shifted sideways (port of CS1 CreateParallel). Ctrl+click on a segment picks the new network's type.</summary>
    public sealed class ParallelMode : NodeChainMode
    {
        private readonly List<Bezier4x3> _edges = new List<Bezier4x3>();
        private List<Point> _points = new List<Point>();
        private Entity _prefab;
        private string _prefabName;
        private float _shift;
        private bool _shiftEdited;
        private bool _left = true;
        private float _height;
        private bool _invert;
        private bool _picked;
        private Entity _pickHover;

        public override ToolModeId Id => ToolModeId.Parallel;

        // Direction follows side, like CS1 (ResultInvert = Side ^ Invert): on the left the new path runs against the chain, as an opposite carriageway in right-hand traffic.
        private bool Reversed => _left != _invert;

        protected override int MinNodes => 2;

        public override string Info
        {
            get
            {
                if (Nodes.Count < 2)
                    return base.Info + "\nCtrl+click a segment: pick its type";
                var status = State != ChainState.None ? base.Info : $"Press {KeyNames.Of(ToolAction.Apply)} to create";
                return $"{status}\n\nType: {_prefabName ?? "?"}\nOffset: {_shift:0.0} m\nHeight: {_height:0.0} m\n\n" +
                       $"Ctrl+click segment: pick type\n{KeyNames.Of(ToolAction.OffsetDown)}/{KeyNames.Of(ToolAction.OffsetUp)}: offset\n{KeyNames.Of(ToolAction.HeightUp)}/{KeyNames.Of(ToolAction.HeightDown)}: height\n" +
                       $"{KeyNames.Of(ToolAction.Side)}: side\n{KeyNames.Of(ToolAction.Invert)}: invert";
            }
        }

        // A picked type survives validation, so several parallels of that type can be built in a row; leaving the mode forgets it.
        public override void Enter(ToolContext ctx)
        {
            _picked = false;
            base.Enter(ctx);
        }

        public override void Exit(ToolContext ctx)
        {
            _picked = false;
            base.Exit(ctx);
        }

        public override void Update(ToolContext ctx)
        {
            // Tested before base.Update, which snaps an edge hover near its ends to a node.
            _pickHover = ctx.CtrlHeld && ctx.HoverIsEdge && ctx.Em.Exists(ctx.Hover) && !ctx.Net.HasOwner(ctx.Hover) ? ctx.Hover : Entity.Null;
            if (ctx.ApplyPressed && _pickHover != Entity.Null)
            {
                _picked = true;
                _prefab = ctx.Net.Prefab(_pickHover);
                _prefabName = ctx.PrefabName(_prefab);
                if (!_shiftEdited)
                    _shift = DefaultShift(ctx);
                ctx.ApplyPressed = false;
            }

            base.Update(ctx);
            if (Nodes.Count < 2)
            {
                _points.Clear();
                return;
            }

            HandleKeys(ctx);
            _points = Solve(ctx);

            var net = ctx.Net;
            var followTerrain = ctx.Settings != null && ctx.Settings.FollowTerrain && _height == 0f;
            foreach (var node in Nodes)
                followTerrain &= math.all(math.abs(net.Elevation(node)) < 1f);

            var template = ChainEdge(ctx, 0);
            var invertTemplate = !net.IsStart(template, Nodes[0]) != Reversed;
            ctx.Emitter().Chain(Entity.Null, Entity.Null, _points, template, invertTemplate, followTerrain, followTerrain ? null : (System.Func<float3, float>)ctx.TerrainHeight, _prefab);
        }

        protected override void OnChainChanged(ToolContext ctx)
        {
            // Below 2 nodes the chain type is unknown: a re-extended chain takes its new edges' type.
            if (Nodes.Count < 2 && !_picked)
            {
                _prefab = Entity.Null;
                _prefabName = null;
            }

            if (Nodes.Count == 0)
            {
                _shift = 0f;
                _shiftEdited = false;
                _left = true;
                _height = 0f;
                _invert = false;
                _points.Clear();
                return;
            }

            if (Nodes.Count < 2)
                return;
            if (_prefab == Entity.Null)
            {
                _prefab = ctx.Net.Prefab(ChainEdge(ctx, 0));
                _prefabName = ctx.PrefabName(_prefab);
            }

            if (!_shiftEdited)
                _shift = DefaultShift(ctx);
        }

        // The preview built from last frame's definitions becomes permanent; the base clears the chain so nothing is emitted this frame.
        protected override void Apply(ToolContext ctx) => ctx.ApplyMode = ApplyMode.Apply;

        protected override void RenderPreview(ToolContext ctx, Overlay overlay)
        {
            if (_points.Count >= 2)
                overlay.Path(_points, ctx.Net.PrefabWidth(_prefab), Overlay.White);
            if (_pickHover != Entity.Null && ctx.Em.Exists(_pickHover))
                overlay.Edge(_pickHover, Overlay.Blue);
        }

        private void HandleKeys(ToolContext ctx)
        {
            if (ctx.Pressed(ToolAction.OffsetUp))
            {
                _shift += ctx.StepMultiplier;
                _shiftEdited = true;
            }

            if (ctx.Pressed(ToolAction.OffsetDown))
            {
                _shift = math.max(_shift - ctx.StepMultiplier, 0f);
                _shiftEdited = true;
            }

            if (ctx.Pressed(ToolAction.HeightUp))
                _height += ctx.StepMultiplier;
            if (ctx.Pressed(ToolAction.HeightDown))
                _height -= ctx.StepMultiplier;
            if (ctx.Pressed(ToolAction.Side))
                _left = !_left;
            if (ctx.Pressed(ToolAction.Invert))
                _invert = !_invert;
        }

        // CS1: mean half width of the chain edges plus half the new type's width.
        private float DefaultShift(ToolContext ctx)
        {
            if (Nodes.Count < 2)
                return 0f;
            var sum = 0f;
            for (var i = 0; i < Nodes.Count - 1; i++)
                sum += ctx.Net.Width(ChainEdge(ctx, i)) / 2f;
            return sum / (Nodes.Count - 1) + ctx.Net.PrefabWidth(_prefab) / 2f;
        }

        // Recomputed every frame: cheap, and follows chain edges changed by other tools.
        private List<Point> Solve(ToolContext ctx)
        {
            var net = ctx.Net;
            _edges.Clear();
            for (var i = 0; i < Nodes.Count - 1; i++)
            {
                var edge = ChainEdge(ctx, i);
                var curve = net.Curve(edge);
                _edges.Add(net.IsStart(edge, Nodes[i]) ? curve : MathUtils.Invert(curve));
            }

            var points = ParallelSolver.Solve(_edges, _shift, _left, _height);
            if (Reversed)
            {
                points.Reverse();
                for (var i = 0; i < points.Count; i++)
                    points[i] = new Point(points[i].Position, -points[i].InDirection, -points[i].Direction);
            }

            return points;
        }
    }
}
