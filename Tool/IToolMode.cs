namespace NetworkMultitool.Tool
{
    /// <summary>One behaviour of the tool. Register it in <c>NetworkMultitoolSystem.OnCreate</c> with <c>RegisterMode</c>.</summary>
    public interface IToolMode
    {
        ToolModeId Id { get; }

        /// <summary>Status text shown in the UI; compared each frame to raise <c>Changed</c>.</summary>
        string Info { get; }

        void Enter(ToolContext ctx);

        void Exit(ToolContext ctx);

        /// <summary>Runs every frame while the tool is active, after old definitions were scheduled for destruction.</summary>
        void Update(ToolContext ctx);

        /// <summary>Draws the overlay; runs right after <see cref="Update"/>.</summary>
        void Render(ToolContext ctx, Overlay overlay);
    }
}
