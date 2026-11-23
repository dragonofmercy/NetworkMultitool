using System.Collections.Generic;
using Colossal;
using NetworkMultitool.Localization;

namespace NetworkMultitool
{
    /// <summary>One game localization source per language, built from <see cref="Texts"/>: options screen IDs plus every mode and UI text under <see cref="Texts.Id"/>.</summary>
    public class Locale : IDictionarySource
    {
        private readonly Settings _settings;
        private readonly Dictionary<string, string> _texts;

        /// <param name="texts">The language's texts; English fills the keys it lacks.</param>
        public Locale(Settings settings, Dictionary<string, string> texts, Dictionary<string, string> english)
        {
            _settings = settings;
            _texts = new Dictionary<string, string>(english);
            foreach (var pair in texts)
                _texts[pair.Key] = pair.Value;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            var entries = new Dictionary<string, string>
            {
                { _settings.GetSettingsLocaleID(), _texts["Settings.Title"] },
                { _settings.GetOptionGroupLocaleID(Settings.GeneralGroup), _texts["Group.General"] },
                { _settings.GetOptionGroupLocaleID(Settings.KeybindingsGroup), _texts["Group.Keybindings"] },
                { _settings.GetBindingMapLocaleID(), _texts["Settings.Title"] },
            };

            foreach (var option in new[] { nameof(Settings.SegmentLength), nameof(Settings.SlopeInDegrees), nameof(Settings.FollowTerrain), nameof(Settings.HideGamePreview) })
            {
                entries[_settings.GetOptionLabelLocaleID(option)] = _texts["Option." + option];
                entries[_settings.GetOptionDescLocaleID(option)] = _texts["OptionDesc." + option];
            }

            // The options page labels each ProxyBinding property; the binding key label is shown in the key map.
            void Bind(string action, string property = null)
            {
                var label = _texts["Binding." + action];
                entries[_settings.GetBindingKeyLocaleID(action)] = label;
                entries[_settings.GetOptionLabelLocaleID(property ?? action)] = label;
            }

            Bind(nameof(Settings.Toggle));
            Bind(nameof(Settings.ModeAddNode));
            Bind(nameof(Settings.ModeRemoveNode));
            Bind(nameof(Settings.ModeSlope));
            Bind(nameof(Settings.ModeArrangeLine));
            Bind(nameof(Settings.ModeLoop));
            Bind(nameof(Settings.ModeConnection));
            Bind(nameof(Settings.ModeCurve));
            Bind(nameof(Settings.ModeParallel));
            Bind(ToolActions.Apply, nameof(Settings.ApplyKey));
            Bind(nameof(Settings.RadiusUp));
            Bind(nameof(Settings.RadiusDown));
            Bind(nameof(Settings.OffsetUp));
            Bind(nameof(Settings.OffsetDown));
            Bind(nameof(Settings.HeightUp));
            Bind(nameof(Settings.HeightDown));
            Bind(nameof(Settings.Side));
            Bind(nameof(Settings.SwitchOffset));

            foreach (var pair in _texts)
                entries[Texts.Id(pair.Key)] = pair.Value;
            return entries;
        }

        public void Unload()
        {
        }
    }
}
