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

        public void Edge(Entity edge, Color color) => _buffer.DrawCurve(color, _net.Curve(edge), _net.Width(edge));

        /// <summary>One curve per consecutive pair, leaving each point along its forward direction: the color at 25% inside an opaque outline.</summary>
        public void Path(IReadOnlyList<Point> points, float width, Color color)
        {
            var outline = new Color(color.r, color.g, color.b, 1f);
            var fill = new Color(color.r, color.g, color.b, 0.25f);
            for (var i = 1; i < points.Count; i++)
                _buffer.DrawCurve(outline, fill, RingWidth, 0, CourseEmitter.Fit(points[i - 1], points[i]), width);
        }

        // Drawn at the given height, not projected: projected shapes are hidden under trees, absolute ones stay visible over them.
        public void CircleOutline(float3 center, float radius, Color color, float width = 1f) => _buffer.DrawCircle(color, Color.clear, width, 0, new float2(0f, 1f), center, radius * 2f);

        /// <summary>A disc at the given height.</summary>
        public void Handle(float3 center, float diameter, Color color) => _buffer.DrawCircle(color, color, 0f, 0, new float2(0f, 1f), center, diameter);

        public void Disc(float3 center, float diameter, Color color) => _buffer.DrawCircle(color, center, diameter);
    }
}
