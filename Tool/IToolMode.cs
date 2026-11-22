namespace NetworkMultitool.Tool
{
    /// <summary>One behaviour of the tool. Register it in <c>NetworkMultitoolSystem.OnCreate</c> with <c>RegisterMode</c>.</summary>
    public interface IToolMode
    {
        ToolModeId Id { get; }

        /// <summary>Text shown in the UI panel (polled by the UI system): a status line, then optionally a block of "Label: value" lines and a block of key hints, blocks separated by a blank line.</summary>
        string Info { get; }

        void Enter(ToolContext ctx);

        void Exit(ToolContext ctx);

        /// <summary>Runs every frame while the tool is active (also over the UI, without mouse input), after old definitions were scheduled for destruction.</summary>
        void Update(ToolContext ctx);

        /// <summary>Draws the overlay; runs right after <see cref="Update"/>.</summary>
        void Render(ToolContext ctx, Overlay overlay);
    }
}
