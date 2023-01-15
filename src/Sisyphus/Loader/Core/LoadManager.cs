using System;
using System.Collections.Generic;
using log4net;
using Sisyphus.Loader.API;
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
    private static List<IMod> Initialize(IModLoader loader) {
        log.Info("Initializing sisyphus...");

        var mods = loader.ResolveMods();

        try {
            mods = loader.SortAndValidateMods(mods);
        }
        catch (Exception e) {
            log.Error("Caught error while sorting and validating mods:", e);
            log.Warn("Failed to initialize sisyphus, aborting mod loading.");
            return new List<IMod>();
        }

        log.Debug("Loading mods in the following order:");
        foreach (var mod in mods)
            log.Debug($"    {mod.Name} ({mod.Metadata.Version})");

        var loadedMods = loader.LoadMods(mods);
        log.Info("Initialized sisyphus!");
        return loadedMods;
    }
}
