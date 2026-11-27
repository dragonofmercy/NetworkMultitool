using System.Collections.Generic;
using Colossal.Mathematics;
using Game.Tools;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using NetworkMultitool.Tool;
using Unity.Entities;
using Unity.Mathematics;
using static NetworkMultitool.Localization.Texts;

namespace NetworkMultitool.Modes
{
    /// <summary>Builds a new network alongside the selected chain, shifted sideways (port of CS1 CreateParallel). Ctrl+click on a segment picks the new network's type.</summary>
    public sealed class ParallelMode : NodeChainMode
    {
        // Gap left between the chain and the new network by default, so their edges and pillars do not touch.
        private const float DefaultGap = 4f;

        private readonly List<Bezier4x3> _edges = new List<Bezier4x3>();
        private List<Point> _points = new List<Point>();
        private bool _followTerrain;
        private Entity _prefab;
        private string _prefabName;
        private float _shift;
        private bool _shiftEdited;
        private bool _left = true;
        private float _height;
        private readonly TypePicker _picker = new TypePicker();
        private int _oneWay;
        private bool _invert;
        private bool _hovering;
        private bool _dragging;
        private float _grabDelta;
        private float _grabY;

        public override ToolModeId Id => ToolModeId.Parallel;

        // Direction follows side, like CS1: on the left the new path runs against the chain, as an opposite carriageway in right-hand traffic.
        // A one-way type can be inverted on top of that.
        private bool Reversed => _left != (_oneWay != 0 && _invert);

        protected override int MinNodes => 2;

        public override string Info
        {
            get
            {
                if (Nodes.Count < 2)
                    return base.Info + "\n" + T("ParallelPickHover");
                var status = State != ChainState.None ? base.Info : T("PressToCreate", KeyNames.Of(ToolAction.Apply));
                return $"{status}\n\n{T("ParallelType")}: {_prefabName ?? "?"}\n{T("Offset")}: {_shift:0.0} m\n{T("Height")}: {_height:0.0} m\n\n" +
                       $"{T("ParallelHintPick")}\n{T("ParallelHintDrag")}\n{T("HintOffset", KeyNames.Of(ToolAction.OffsetDown), KeyNames.Of(ToolAction.OffsetUp))}\n{T("HintHeight", KeyNames.Of(ToolAction.HeightUp), KeyNames.Of(ToolAction.HeightDown))}\n" +
                       T("ParallelHintSide", KeyNames.Of(ToolAction.SwitchOffset)) + (_oneWay != 0 ? "\n" + T("HintInvert", KeyNames.Of(ToolAction.Invert)) : string.Empty);
            }
        }

        // A picked type survives validation, so several parallels of that type can be built in a row; leaving the mode forgets it.
        public override void Enter(ToolContext ctx)
        {
            _picker.Clear();
            base.Enter(ctx);
        }

        public override void Exit(ToolContext ctx)
        {
            _picker.Clear();
            base.Exit(ctx);
        }

        public override void Update(ToolContext ctx)
        {
            // Tested before base.Update, which snaps an edge hover near its ends to a node.
            if (_picker.Update(ctx))
            {
                _prefab = _picker.Prefab;
                _prefabName = _picker.Name;
                if (!_shiftEdited)
                    _shift = DefaultShift(ctx);
            }

            base.Update(ctx);
            if (Nodes.Count < 2)
            {
                _points.Clear();
                _hovering = false;
                _dragging = false;
                return;
            }

            _oneWay = ctx.Net.OneWay(_prefab);
            HandleKeys(ctx);
            _points = Solve(ctx);
            if (UpdateDrag(ctx))
                _points = Solve(ctx);

            var net = ctx.Net;
            var followTerrain = ctx.Settings != null && ctx.Settings.FollowTerrain && _height == 0f;
            foreach (var node in Nodes)
                followTerrain &= math.all(math.abs(net.Elevation(node)) < 1f);
            _followTerrain = followTerrain;

            // A picked segment also gives its upgrades (bike lanes, trees...); its direction has no relation to the path, so they are not swapped.
            var chainEdge = ChainEdge(ctx, 0);
            var template = _picker.Template(ctx, chainEdge);
            var invertTemplate = template == chainEdge && !net.IsStart(chainEdge, Nodes[0]) != Reversed;
            ctx.Emitter().Chain(Entity.Null, Entity.Null, _points, template, invertTemplate, followTerrain, followTerrain ? null : (System.Func<float3, float>)ctx.TerrainHeight, _prefab);
        }

        protected override void OnChainChanged(ToolContext ctx)
        {
            // Below 2 nodes the chain type is unknown: a re-extended chain takes its new edges' type.
            if (Nodes.Count < 2 && !_picker.Picked)
            {
                _prefab = Entity.Null;
                _prefabName = null;
            }

            _dragging = false;
            if (Nodes.Count == 0)
            {
                _shift = 0f;
                _shiftEdited = false;
                _left = true;
                _invert = false;
                _height = 0f;
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
            {
                var ground = _followTerrain ? ctx.TerrainHeight : (System.Func<float3, float>)null;
                var width = ctx.Net.PrefabWidth(_prefab);
                overlay.Path(_points, width, Overlay.Orange, ground, true, ctx.GamePreviewShown, _oneWay);
                if (_hovering || _dragging)
                    overlay.Path(_points, width + 0.7f, Overlay.Green, ground, true);
            }

            _picker.Render(ctx, overlay);
        }

        // Left press on the preview grabs it; while held, the cursor's signed distance to the chain sets side and offset. Returns true when the offset changed and the preview must be solved again.
        private bool UpdateDrag(ToolContext ctx)
        {
            if (!ctx.ApplyHeld)
                _dragging = false;

            _hovering = false;
            if (!_dragging && ctx.HasHit && !ctx.CtrlHeld && State == ChainState.None && _points.Count >= 2)
            {
                // Each piece is tested where the mouse ray crosses its own height: an elevated preview is not above the terrain hit.
                var half = ctx.Net.PrefabWidth(_prefab) / 2f;
                for (var i = 0; i < _points.Count - 1 && !_hovering; i++)
                {
                    var y = (_points[i].Position.y + _points[i + 1].Position.y) / 2f;
                    if (math.abs(ParallelSolver.SignedOffset(CourseEmitter.Fit(_points[i], _points[i + 1]), ctx.MouseAtHeight(y))) <= half)
                    {
                        _hovering = true;
                        _grabY = y;
                    }
                }
            }

            if (_hovering && ctx.ApplyPressed)
            {
                _dragging = true;
                _grabDelta = ParallelSolver.SignedOffset(_edges, ctx.MouseAtHeight(_grabY)) - (_left ? _shift : -_shift);
            }

            if (!_dragging || !ctx.HasHit)
                return false;
            // The grab height stays fixed during the drag, so the preview follows the cursor at the height it was taken.
            var signed = ParallelSolver.SignedOffset(_edges, ctx.MouseAtHeight(_grabY)) - _grabDelta;
            _left = signed >= 0f;
            _shift = math.abs(signed);
            _shiftEdited = true;
            return true;
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
            if (ctx.Pressed(ToolAction.SwitchOffset))
                _left = !_left;
            if (_oneWay != 0 && ctx.Pressed(ToolAction.Invert))
                _invert = !_invert;
        }

        // CS1: mean half width of the chain edges plus half the new type's width, plus a gap.
        private float DefaultShift(ToolContext ctx)
        {
            if (Nodes.Count < 2)
                return 0f;
            var sum = 0f;
            for (var i = 0; i < Nodes.Count - 1; i++)
                sum += ctx.Net.Width(ChainEdge(ctx, i)) / 2f;
            return sum / (Nodes.Count - 1) + ctx.Net.PrefabWidth(_prefab) / 2f + DefaultGap;
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
                    points[i] = points[i].Reversed();
            }

            return points;
        }
    }
}
