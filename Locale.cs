using System.Collections.Generic;
using Colossal;

namespace NetworkMultitool
{
    public class Locale : IDictionarySource
    {
        private readonly Settings _settings;

        public Locale(Settings settings)
        {
            _settings = settings;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            var entries = new Dictionary<string, string>
            {
                { _settings.GetSettingsLocaleID(), "Network Multitool" },
                { _settings.GetOptionGroupLocaleID(Settings.GeneralGroup), "General" },
                { _settings.GetOptionGroupLocaleID(Settings.KeybindingsGroup), "Keybindings" },
                { _settings.GetBindingMapLocaleID(), "Network Multitool" },

                { _settings.GetOptionLabelLocaleID(nameof(Settings.SegmentLength)), "Segment length" },
                { _settings.GetOptionDescLocaleID(nameof(Settings.SegmentLength)), "Target length (in meters) of the road segments created by the tool." },
                { _settings.GetOptionLabelLocaleID(nameof(Settings.SlopeInDegrees)), "Show slope in degrees" },
                { _settings.GetOptionDescLocaleID(nameof(Settings.SlopeInDegrees)), "Display slopes in degrees instead of percent." },
                { _settings.GetOptionLabelLocaleID(nameof(Settings.FollowTerrain)), "Follow terrain" },
                { _settings.GetOptionDescLocaleID(nameof(Settings.FollowTerrain)), "Let created roads follow the terrain height instead of staying flat." },
                { _settings.GetOptionLabelLocaleID(nameof(Settings.HideGamePreview)), "Hide game preview" },
                { _settings.GetOptionDescLocaleID(nameof(Settings.HideGamePreview)), "Hide the game's road preview in the creation modes and keep only the mod's overlay. Cost and validation still apply." },
            };

            // The options page labels each ProxyBinding property; the binding key label is shown in the key map.
            void Bind(string action, string label, string property = null)
            {
                entries[_settings.GetBindingKeyLocaleID(action)] = label;
                entries[_settings.GetOptionLabelLocaleID(property ?? action)] = label;
            }

            Bind(nameof(Settings.Toggle), "Toggle tool");
            Bind(nameof(Settings.ModeAddNode), "Mode: add node");
            Bind(nameof(Settings.ModeRemoveNode), "Mode: remove node");
            Bind(nameof(Settings.ModeSlope), "Mode: slope");
            Bind(nameof(Settings.ModeArrangeLine), "Mode: arrange");
            Bind(nameof(Settings.ModeLoop), "Mode: loop");
            Bind(nameof(Settings.ModeConnection), "Mode: connection");
            Bind(nameof(Settings.ModeCurve), "Mode: curve");
            Bind(nameof(Settings.ModeParallel), "Mode: parallel");
            Bind(ToolActions.Apply, "Apply", nameof(Settings.ApplyKey));
            Bind(nameof(Settings.RadiusUp), "Increase radius");
            Bind(nameof(Settings.RadiusDown), "Decrease radius");
            Bind(nameof(Settings.OffsetUp), "Increase offset");
            Bind(nameof(Settings.OffsetDown), "Decrease offset");
            Bind(nameof(Settings.HeightUp), "Raise height");
            Bind(nameof(Settings.HeightDown), "Lower height");
            Bind(nameof(Settings.Side), "Switch side");
            Bind(nameof(Settings.SwitchOffset), "Switch offset");

            return entries;
        }

        public void Unload()
        {
        }
    }
}
