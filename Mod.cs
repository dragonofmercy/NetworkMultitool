using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

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
            GameManager.instance.localizationManager.AddSource("en-US", new Locale(Settings));
            AssetDatabase.global.LoadSettings(nameof(NetworkMultitool), Settings, new Settings(this));
        }

        public void OnDispose()
        {
            Settings?.UnregisterInOptionsUI();
        }
    }
}
