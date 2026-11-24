using System.Collections.Generic;
using Game.Rendering;
using NetworkMultitool.Geometry;
using NetworkMultitool.Net;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace NetworkMultitool.Tool
{
    /// <summary>Main-thread drawing helpers over the overlay buffer, valid for one frame.</summary>
    public readonly struct Overlay
    {
        private const float DefaultNodeWidth = 8f;
        private const float RingWidth = 1f;
        private const float EdgePieceLength = 16f;
        private static readonly Color MarkerFill = new Color(1f, 1f, 1f, 0.15f);

        public static readonly Color White = new Color(1f, 1f, 1f, 0.6f);
        public static readonly Color Green = new Color(0f, 1f, 0f, 0.6f);
        public static readonly Color Red = new Color(1f, 0f, 0f, 0.6f);
        public static readonly Color Yellow = new Color(1f, 1f, 0f, 0.6f);
        public static readonly Color Purple = new Color(0.6f, 0f, 1f, 0.6f);
        public static readonly Color Blue = new Color(0f, 0.4f, 1f, 0.6f);
        /// <summary>CS1 CommonColors.Yellow, the color of its path preview.</summary>
        public static readonly Color Orange = new Color(1f, 0.733f, 0.2f, 0.6f);
        public static readonly Color Gray = new Color(0.8f, 0.8f, 0.8f, 0.6f);

        private readonly OverlayRenderSystem.Buffer _buffer;
        private readonly NetQuery _net;
        private readonly List<(float3 Position, float3 Direction, string Text)> _labels;

        public Overlay(OverlayRenderSystem.Buffer buffer, NetQuery net, List<(float3, float3, string)> labels)
        {
            _buffer = buffer;
            _net = net;
            _labels = labels;
        }

        /// <summary>Text shown by the UI at the screen position of a world point; lines are separated by '\n'.</summary>
        /// <param name="direction">World direction the text extends to from the point (CS1 InfoLabel.Direction).</param>
        public void Label(float3 position, float3 direction, string text) => _labels.Add((position, direction, text));

        /// <summary>A thin line at the given height.</summary>
        public void GroundLine(float3 a, float3 b, Color color, float width = 1f) => _buffer.DrawLine(color, color, 0f, 0, new Colossal.Mathematics.Line3.Segment(a, b), width, new float2(0f, 0f));

        /// <summary>A disc on the node, as wide as its first connected edge.</summary>
        public void Node(Entity node, Color color) => _buffer.DrawCircle(color, _net.NodePosition(node), NodeWidth(node));

        /// <summary>Faint disc with a ring: marks where a node is without hiding the mode's own highlights.</summary>
        public void NodeMarker(Entity node, float3 position) => _buffer.DrawCircle(White, MarkerFill, RingWidth, 0, new float2(0f, 1f), position, NodeWidth(node));

        private float NodeWidth(Entity node)
        {
            var edge = _net.FirstEdge(node);
            return edge != Entity.Null ? _net.Width(edge) : DefaultNodeWidth;
        }

        /// <summary>The edge in short pieces, each with a constant slope: an absolute overlay curve lies in the plane of its control points,
        /// so a segment curved only vertically (over a crest) would be drawn as a band standing on its edge (OverlayRenderSystem.FitQuad).</summary>
        public void Edge(Entity edge, Color color)
        {
            var curve = _net.Curve(edge);
            var width = _net.Width(edge);
            var count = math.max(1, (int)math.ceil(Colossal.Mathematics.MathUtils.Length(curve) / EdgePieceLength));
            for (var i = 0; i < count; i++)
                _buffer.DrawCurve(color, Vec.ConstantSlope(Colossal.Mathematics.MathUtils.Cut(curve, new float2(i, i + 1) / count)), width);
        }

        /// <summary>One curve per consecutive pair, leaving each point along its forward direction: the color at 25% inside an opaque outline.</summary>
        /// <param name="ground">With Follow terrain the game puts every created node on the terrain (CoursePosFlags.FreeHeight): the points are drawn there too.</param>
        /// <param name="groundEnds">False when the ends are existing nodes, which keep their height.</param>
        /// <param name="faint">Outline only, half transparent: the game's own preview is drawn under it and must stay visible.</param>
        public void Path(IReadOnlyList<Point> points, float width, Color color, System.Func<float3, float> ground = null, bool groundEnds = false, bool faint = false)
        {
            points = Grounded(points, ground, groundEnds);
            var outline = new Color(color.r, color.g, color.b, faint ? 0.5f : 1f);
            var fill = faint ? Color.clear : new Color(color.r, color.g, color.b, 0.25f);
            for (var i = 1; i < points.Count; i++)
                _buffer.DrawCurve(outline, fill, RingWidth, 0, Vec.ConstantSlope(CourseEmitter.Fit(points[i - 1], points[i])), width);
        }

        /// <summary>Chevrons along a path showing the traffic direction of a one-way road: <paramref name="direction"/> +1 along the point order, -1 against it.</summary>
        public void Arrows(IReadOnlyList<Point> points, float width, int direction, Color color, System.Func<float3, float> ground = null, bool groundEnds = false)
        {
            points = Grounded(points, ground, groundEnds);
            var size = math.clamp(width * 0.3f, 1.5f, 4f);
            for (var i = 1; i < points.Count; i++)
            {
                var curve = Vec.ConstantSlope(CourseEmitter.Fit(points[i - 1], points[i]));
                var count = math.max(1, (int)math.round(Colossal.Mathematics.MathUtils.Length(curve) / ArrowSpacing));
                for (var k = 0; k < count; k++)
                {
                    var t = (k + 0.5f) / count;
                    var tip = Colossal.Mathematics.MathUtils.Position(curve, t);
                    var forward = Vec.NormalizeXZ(Colossal.Mathematics.MathUtils.Tangent(curve, t)) * direction;
                    var side = Vec.Turn90(forward, true);
                    var back = tip - forward * size;
                    GroundLine(back + side * size, tip, color, 0.5f);
                    GroundLine(back - side * size, tip, color, 0.5f);
                }
            }
        }

        private const float ArrowSpacing = 20f;

        private static IReadOnlyList<Point> Grounded(IReadOnlyList<Point> points, System.Func<float3, float> ground, bool groundEnds)
        {
            if (ground == null)
                return points;
            var grounded = new List<Point>(points.Count);
            for (var i = 0; i < points.Count; i++)
            {
                var p = points[i];
                grounded.Add(!groundEnds && (i == 0 || i == points.Count - 1) ? p : new Point(Vec.WithY(p.Position, ground(p.Position)), p.Direction, p.InDirection));
            }

            return grounded;
        }

        // Drawn at the given height, not projected: projected shapes are hidden under trees, absolute ones stay visible over them.
        public void CircleOutline(float3 center, float radius, Color color, float width = 1f) => _buffer.DrawCircle(color, Color.clear, width, 0, new float2(0f, 1f), center, radius * 2f);

        /// <summary>A disc at the given height.</summary>
        public void Handle(float3 center, float diameter, Color color) => _buffer.DrawCircle(color, color, 0f, 0, new float2(0f, 1f), center, diameter);

        public void Disc(float3 center, float diameter, Color color) => _buffer.DrawCircle(color, center, diameter);
    }
}
