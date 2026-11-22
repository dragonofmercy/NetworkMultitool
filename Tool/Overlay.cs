using System.Collections.Generic;
using Colossal.Mathematics;
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
        private const int NearbyHops = 3;

        public static readonly Color White = new Color(1f, 1f, 1f, 0.6f);
        public static readonly Color Green = new Color(0f, 1f, 0f, 0.6f);
        public static readonly Color Red = new Color(1f, 0f, 0f, 0.6f);
        public static readonly Color Yellow = new Color(1f, 1f, 0f, 0.6f);
        public static readonly Color Purple = new Color(0.6f, 0f, 1f, 0.6f);
        public static readonly Color Blue = new Color(0f, 0.4f, 1f, 0.6f);
        public static readonly Color Gray = new Color(0.8f, 0.8f, 0.8f, 0.6f);
        public static readonly Color Orange = new Color(1f, 0.5f, 0f, 0.6f);

        private readonly OverlayRenderSystem.Buffer _buffer;
        private readonly NetQuery _net;

        public Overlay(OverlayRenderSystem.Buffer buffer, NetQuery net)
        {
            _buffer = buffer;
            _net = net;
        }

        /// <summary>A disc on the node, as wide as its first connected edge.</summary>
        public void Node(Entity node, Color color) => _buffer.DrawCircle(color, _net.NodePosition(node), NodeWidth(node));

        /// <summary>Rings on the nodes around the hovered node or edge, so the pickable nodes show up before the cursor reaches them.</summary>
        public void NearbyNodes(Entity hover, Color color)
        {
            foreach (var node in _net.Nearby(hover, NearbyHops))
                _buffer.DrawCircle(color, Color.clear, RingWidth, 0, new float2(0f, 1f), _net.NodePosition(node), NodeWidth(node));
        }

        private float NodeWidth(Entity node)
        {
            foreach (var edge in _net.Edges(node))
                return _net.Width(edge);
            return DefaultNodeWidth;
        }

        public void Edge(Entity edge, Color color) => _buffer.DrawCurve(color, _net.Curve(edge), _net.Width(edge));

        /// <summary>One curve per consecutive pair, leaving each point along its forward direction.</summary>
        public void Path(IReadOnlyList<Point> points, float width, Color color)
        {
            for (var i = 1; i < points.Count; i++)
                _buffer.DrawCurve(color, CourseEmitter.Fit(points[i - 1], points[i]), width);
        }

        public void Curve(Bezier4x3 bezier, float width, Color color) => _buffer.DrawCurve(color, bezier, width);

        public void Line(float3 a, float3 b, float width, Color color) => _buffer.DrawLine(color, new Line3.Segment(a, b), width);

        public void CircleOutline(float3 center, float radius, Color color) => _buffer.DrawCircle(color, Color.clear, 1f, 0, new float2(0f, 1f), center, radius * 2f);

        public void Disc(float3 center, float diameter, Color color) => _buffer.DrawCircle(color, center, diameter);
    }
}
