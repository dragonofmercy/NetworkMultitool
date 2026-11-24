using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace NetworkMultitool
{
    // The action name doubles as the ProxyBinding property name (the locale uses nameof on the property), except Apply whose property is ApplyKey.
    public static class ToolActions
    {
        public const string Toggle = nameof(Toggle);
        public const string ModeAddNode = nameof(ModeAddNode);
        public const string ModeRemoveNode = nameof(ModeRemoveNode);
        public const string ModeSlope = nameof(ModeSlope);
        public const string ModeArrangeLine = nameof(ModeArrangeLine);
        public const string ModeLoop = nameof(ModeLoop);
        public const string ModeConnection = nameof(ModeConnection);
        public const string ModeCurve = nameof(ModeCurve);
        public const string ModeParallel = nameof(ModeParallel);
        public const string Apply = nameof(Apply);
        public const string RadiusUp = nameof(RadiusUp);
        public const string RadiusDown = nameof(RadiusDown);
        public const string OffsetUp = nameof(OffsetUp);
        public const string OffsetDown = nameof(OffsetDown);
        public const string HeightUp = nameof(HeightUp);
        public const string HeightDown = nameof(HeightDown);
        public const string Side = nameof(Side);
        public const string Invert = nameof(Invert);
        public const string SwitchOffset = nameof(SwitchOffset);
    }

    [FileLocation("ModsSettings\\NetworkMultitool\\NetworkMultitool")]
    [SettingsUIGroupOrder(GeneralGroup, KeybindingsGroup)]
    [SettingsUIShowGroupName(GeneralGroup, KeybindingsGroup)]
    [SettingsUIKeyboardAction(ToolActions.Toggle, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeAddNode, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeRemoveNode, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeSlope, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeArrangeLine, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeLoop, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeConnection, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeCurve, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.ModeParallel, Usages.kDefaultUsage, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.Apply, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.RadiusUp, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.RadiusDown, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.OffsetUp, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.OffsetDown, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.HeightUp, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.HeightDown, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.Side, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.Invert, Usages.kToolUsage)]
    [SettingsUIKeyboardAction(ToolActions.SwitchOffset, Usages.kToolUsage)]
    public class Settings : ModSetting
    {
        public const string GeneralGroup = "General";
        public const string KeybindingsGroup = "Keybindings";

        [SettingsUISection(GeneralGroup)]
        [SettingsUISlider(min = 20f, max = 200f, step = 1f, scalarMultiplier = 1f, unit = Unit.kLength)]
        public float SegmentLength { get; set; }

        [SettingsUISection(GeneralGroup)]
        public bool SlopeInDegrees { get; set; }

        [SettingsUISection(GeneralGroup)]
        public bool FollowTerrain { get; set; }

        [SettingsUISection(GeneralGroup)]
        public bool HideGamePreview { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.T, ToolActions.Toggle, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding Toggle { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit1, ToolActions.ModeAddNode, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeAddNode { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit2, ToolActions.ModeRemoveNode, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeRemoveNode { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit3, ToolActions.ModeSlope, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeSlope { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit4, ToolActions.ModeArrangeLine, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeArrangeLine { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit6, ToolActions.ModeLoop, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeLoop { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit5, ToolActions.ModeConnection, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeConnection { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit7, ToolActions.ModeCurve, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeCurve { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Digit8, ToolActions.ModeParallel, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ModeParallel { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Enter, ToolActions.Apply)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding ApplyKey { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.UpArrow, ToolActions.RadiusUp)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding RadiusUp { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.DownArrow, ToolActions.RadiusDown)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding RadiusDown { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.RightArrow, ToolActions.OffsetUp)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding OffsetUp { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.LeftArrow, ToolActions.OffsetDown)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding OffsetDown { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.PageUp, ToolActions.HeightUp)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding HeightUp { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.PageDown, ToolActions.HeightDown)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding HeightDown { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.S, ToolActions.Side, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding Side { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.I, ToolActions.Invert, ctrl: true, alt: true)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding Invert { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.Tab, ToolActions.SwitchOffset)]
        [SettingsUISection(KeybindingsGroup)]
        public ProxyBinding SwitchOffset { get; set; }

        public Settings(IMod mod) : base(mod)
        {
            // ModSetting does not call it; loaded values override these defaults.
            SetDefaults();
        }

        public override void SetDefaults()
        {
            SegmentLength = 80f;
            SlopeInDegrees = false;
            FollowTerrain = true;
            HideGamePreview = true;
        }
    }
}
