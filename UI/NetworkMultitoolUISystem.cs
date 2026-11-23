using System.Globalization;
using System.Text;
using Colossal.UI.Binding;
using Game.UI;
using NetworkMultitool.Tool;
using UnityEngine;

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
            AddUpdateBinding(new GetterValueBinding<string>(Group, "labels", () => ScreenLabels(tool)));
            AddUpdateBinding(new GetterValueBinding<string>(Group, "toggleKeys", () => KeyNames.Of(ToolActions.Toggle)));

            AddUpdateBinding(new GetterValueBinding<bool>(Group, "followTerrain", () => Mod.Settings != null && Mod.Settings.FollowTerrain));

            AddBinding(new TriggerBinding(Group, "toggle", tool.Toggle));
            AddBinding(new TriggerBinding(Group, "toggleFollowTerrain", ToggleFollowTerrain));
            AddBinding(new TriggerBinding<int>(Group, "setMode", id => tool.SetMode((ToolModeId)id)));
            AddBinding(new TriggerBinding(Group, "apply", tool.RequestApply));
        }

        // One label per line: viewport x, viewport y (from the top), then the text lines, tab-separated. Points behind the camera are dropped.
        private static string ScreenLabels(NetworkMultitoolSystem tool)
        {
            var camera = Camera.main;
            if (!tool.IsActive || camera == null || tool.Labels.Count == 0)
                return "";
            var sb = new StringBuilder();
            foreach (var (position, text) in tool.Labels)
            {
                var p = camera.WorldToViewportPoint(position);
                if (p.z <= 0f)
                    continue;
                sb.Append(p.x.ToString("0.0000", CultureInfo.InvariantCulture)).Append('\t').Append((1f - p.y).ToString("0.0000", CultureInfo.InvariantCulture)).Append('\t').Append(text.Replace('\n', '\t')).Append('\n');
            }
            return sb.ToString();
        }

        // Same setting as the options screen; the creation modes read it every frame.
        private static void ToggleFollowTerrain()
        {
            var settings = Mod.Settings;
            if (settings == null)
                return;
            settings.FollowTerrain = !settings.FollowTerrain;
            settings.ApplyAndSave();
        }
    }
}
