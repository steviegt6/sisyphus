using log4net;
using Sisyphus.Loader.Core.Support;

namespace Sisyphus.Loader.Core;

/// <summary>
///     Handles loading mods for sisyphus and initializes other mod loaders.
/// </summary>
internal static class LoadManager {
    private static readonly ILog log = LogManager.GetLogger("LoadManager");
    
    /// <summary>
    ///     Searches for sisyphus mods and loads them, as well as other mod
    ///     loaders and initializes and loads them as well.
    /// </summary>
    /// <param name="loaderType"></param>
    public static void Load(ref LoaderType loaderType) {
        Initialize();

        BepInEx.Initialize(ref loaderType);
        MelonLoader.Initialize(ref loaderType);
    }

    /// <summary>
    ///     Initializes sisyphus' mod loader, providing assembly pre-patching,
    ///     and other mod loader initialization shenanigans.
    /// </summary>
    private static void Initialize() {
        log.Info("Initializing sisyphus...");

        log.Info("Initialized sisyphus!");
    }
}
