using System.Collections.Generic;

namespace NetworkMultitool.Net
{
    /// <summary>Minimal read-only view of a node graph, so chain logic can be tested without ECS.</summary>
    public interface INetGraph<T>
    {
        int Degree(T node);
        IEnumerable<T> Neighbors(T node);
    }
}
