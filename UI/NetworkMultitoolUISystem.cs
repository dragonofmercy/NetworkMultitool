using System.Collections.Generic;
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
            AddUpdateBinding(new GetterValueBinding<string>(Group, "toggleKeys", ToggleKeys));

            AddBinding(new TriggerBinding(Group, "toggle", tool.Toggle));
            AddBinding(new TriggerBinding<int>(Group, "setMode", id => tool.SetMode((ToolModeId)id)));
            AddBinding(new TriggerBinding(Group, "apply", tool.RequestApply));
        }

        // Current toggle shortcut as "Ctrl+T", for the key chips of the tool button tooltip; empty when unbound.
        private static string ToggleKeys()
        {
            var binding = Mod.Settings?.Toggle;
            if (binding == null || !binding.Value.isSet)
                return "";

            var keys = new List<string>();
            foreach (var modifier in binding.Value.modifiers)
                keys.Add(KeyName(modifier.m_Path));
            keys.Add(KeyName(binding.Value.path));
            return string.Join("+", keys);
        }

        // "<Keyboard>/ctrl" gives "Ctrl".
        private static string KeyName(string path)
        {
            var name = path.Substring(path.LastIndexOf('/') + 1);
            return name.Length == 0 ? name : char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }
}
