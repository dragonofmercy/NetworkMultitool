namespace NetworkMultitool.Geometry
{
    public static class SlopeSolver
    {
        /// <summary>Heights at each node, interpolated linearly by cumulative segment length. Zero total length gives startY everywhere.</summary>
        public static float[] Heights(float startY, float endY, float[] segmentLengths)
        {
            var total = 0f;
            foreach (var l in segmentLengths)
            {
                total += l;
            }
            var heights = new float[segmentLengths.Length + 1];
            heights[0] = startY;
            var acc = 0f;
            for (var i = 0; i < segmentLengths.Length; i += 1)
            {
                acc += segmentLengths[i];
                heights[i + 1] = total > 0f ? startY + (endY - startY) * (acc / total) : startY;
            }
            return heights;
        }
    }
}
