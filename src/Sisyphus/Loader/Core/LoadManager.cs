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
    /// <param name="loader">The sisyphus mod loader engine to use.</param>
    public static void Load(ref LoaderType loaderType, IModLoader loader) {
        Initialize(loader);

        BepInEx.Initialize(ref loaderType);
        MelonLoader.Initialize(ref loaderType);
    }

    /// <summary>
    ///     Initializes sisyphus' mod loader, providing assembly pre-patching,
    ///     and other mod loader initialization shenanigans.
    /// </summary>
    private static void Initialize(IModLoader loader) {
        log.Info("Initializing sisyphus...");

        _ = loader.ResolveMods();

        log.Info("Initialized sisyphus!");
    }
}
