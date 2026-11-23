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
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "hidePreview", () => Mod.Settings != null && Mod.Settings.HideGamePreview));

            AddBinding(new TriggerBinding(Group, "toggle", tool.Toggle));
            AddBinding(new TriggerBinding(Group, "toggleFollowTerrain", () => ToggleSetting(s => s.FollowTerrain = !s.FollowTerrain)));
            AddBinding(new TriggerBinding(Group, "toggleHidePreview", () => ToggleSetting(s => s.HideGamePreview = !s.HideGamePreview)));
            AddBinding(new TriggerBinding<int>(Group, "setMode", id => tool.SetMode((ToolModeId)id)));
            AddBinding(new TriggerBinding(Group, "apply", tool.RequestApply));
        }

        // One label per line: viewport x, viewport y (from the top), screen direction x, y (y down), then the text lines, tab-separated. Points behind the camera are dropped.
        private static string ScreenLabels(NetworkMultitoolSystem tool)
        {
            var camera = Camera.main;
            if (!tool.IsActive || camera == null || tool.Labels.Count == 0)
                return "";
            var sb = new StringBuilder();
            foreach (var (position, direction, text) in tool.Labels)
            {
                var p = camera.WorldToViewportPoint(position);
                if (p.z <= 0f)
                    continue;
                // Direction on screen in pixels, so the text box can be pushed off the point on that side.
                var d = (Vector2)(camera.WorldToScreenPoint((Vector3)(position + direction)) - camera.WorldToScreenPoint(position));
                d = d.sqrMagnitude > 1e-6f ? d.normalized : Vector2.zero;
                sb.Append(F(p.x)).Append('\t').Append(F(1f - p.y)).Append('\t').Append(F(d.x)).Append('\t').Append(F(-d.y)).Append('\t').Append(text.Replace('\n', '\t')).Append('\n');
            }
            return sb.ToString();
        }

        private static string F(float value) => value.ToString("0.0000", CultureInfo.InvariantCulture);

        // Same settings as the options screen; the creation modes read them every frame.
        private static void ToggleSetting(System.Action<Settings> toggle)
        {
            var settings = Mod.Settings;
            if (settings == null)
                return;
            toggle(settings);
            settings.ApplyAndSave();
        }
    }
}
