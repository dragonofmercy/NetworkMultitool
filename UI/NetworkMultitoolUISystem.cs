using Colossal.UI.Binding;
using Game.UI;
using NetworkMultitool.Tool;

namespace NetworkMultitool.UI
{
    /// <summary>Exposes the tool state and commands to the React UI under the "NetworkMultitool" binding group.</summary>
    public partial class NetworkMultitoolUISystem : UISystemBase
    {
        private const string Group = "NetworkMultitool";

        protected override void OnCreate()
        {
            base.OnCreate();
            var tool = World.GetOrCreateSystemManaged<NetworkMultitoolSystem>();

            // Polled every UI update, so changes raised between frames are picked up without the Changed event.
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "active", () => tool.IsActive));
            AddUpdateBinding(new GetterValueBinding<int>(Group, "mode", () => (int)tool.Mode));
            AddUpdateBinding(new GetterValueBinding<string>(Group, "info", () => tool.Info ?? ""));
            AddUpdateBinding(new GetterValueBinding<string>(Group, "toggleKeys", () => KeyNames.Of(ToolActions.Toggle)));

            AddBinding(new TriggerBinding(Group, "toggle", tool.Toggle));
            AddBinding(new TriggerBinding<int>(Group, "setMode", id => tool.SetMode((ToolModeId)id)));
            AddBinding(new TriggerBinding(Group, "apply", tool.RequestApply));
        }
    }
}
