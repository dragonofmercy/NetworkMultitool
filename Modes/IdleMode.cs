using NetworkMultitool.Tool;

namespace NetworkMultitool.Modes
{
    /// <summary>Default mode: the tool is active but does nothing until a mode is picked.</summary>
    public sealed class IdleMode : IToolMode
    {
        public ToolModeId Id => ToolModeId.None;

        public string Info => "Select a mode";

        public void Enter(ToolContext ctx)
        {
        }

        public void Exit(ToolContext ctx)
        {
        }

        public void Update(ToolContext ctx)
        {
        }

        public void Render(ToolContext ctx, Overlay overlay)
        {
        }
    }
}
