namespace NetworkMultitool.Tool
{
    /// <summary>Tool modes; values are shared with the UI. Each non-None id has a matching "Mode" + name key binding.</summary>
    public enum ToolModeId
    {
        None = 0,
        AddNode,
        RemoveNode,
        Slope,
        Loop,
        Connection,
        Curve,
        ArrangeLine,
    }
}
