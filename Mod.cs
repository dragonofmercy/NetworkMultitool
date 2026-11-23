using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using NetworkMultitool.Localization;
using NetworkMultitool.Tool;
using NetworkMultitool.UI;

namespace NetworkMultitool
{
    public class Mod : IMod
    {
        public static Settings Settings { get; private set; }

        public static readonly ILog Log = LogManager.GetLogger($"{nameof(NetworkMultitool)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                Log.Info($"{nameof(Mod)}.{nameof(OnLoad)} Current mod asset at {asset.path}");

            Settings = new Settings(this);
            Settings.RegisterKeyBindings();
            Settings.RegisterInOptionsUI();
            var locales = Texts.ByLocale;
            foreach (var pair in locales)
                GameManager.instance.localizationManager.AddSource(pair.Key, new Locale(Settings, pair.Value, locales["en-US"]));
            AssetDatabase.global.LoadSettings(nameof(NetworkMultitool), Settings, new Settings(this));

            updateSystem.UpdateAt<NetworkMultitoolSystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateAt<NetworkMultitoolUISystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            Settings?.UnregisterInOptionsUI();
        }
    }
}
