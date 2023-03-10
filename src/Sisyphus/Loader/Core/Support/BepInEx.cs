using System;
using System.IO;
using System.Reflection;
using log4net;

namespace Sisyphus.Loader.Core.Support;

/// <summary>
///     BepInEx support for sisyphus.
/// </summary>
internal static class BepInEx {
    private static readonly ILog logger =
        LogManager.GetLogger("Support.BepInEx");

    private const string ep_name = "BepInEx.Preloader.Entrypoint";
    private const string main_name = "Main";

    internal static void Initialize(ref LoaderType loaderType) {
        logger.Info("Initializing BepInEx support...");

        MoveBepInExFolder();

        var expectedDir = SISYPHUS_BEPINEX_CORE_PATH;

        if (!Directory.Exists(expectedDir)) {
            logger.Info("Skipping, not found: " + expectedDir);
            return;
        }

        logger.Info("Found: " + expectedDir);

        Assembly asm;

        try {
            logger.Debug("Loading assembly: " + BEPINEX_PRELOADER_PATH);
            asm = Assembly.LoadFile(BEPINEX_PRELOADER_PATH);
        }
        catch (Exception e) {
            logger.Error("Failed to load BepInEx preloader:", e);
            return;
        }

        logger.Debug("Resolving entrypoint type: " + ep_name);
        var entrypoint = asm.GetType(ep_name);

        if (entrypoint is null) {
            logger.Error("Failed to resolve entrypoint type: " + ep_name);
            return;
        }

        logger.Debug("Resolving entrypoint method: " + main_name);
        var main = entrypoint.GetMethod(
            main_name,
            BindingFlags.Static | BindingFlags.Public
        );

        if (main is null) {
            logger.Error("Failed to resolve entrypoint method: " + main_name);
            return;
        }

        var paramCount = main.GetParameters().Length;

        if (paramCount > 0) {
            logger.Error("Preloader has unexpected param count:" + paramCount);
            return;
        }

        // Flag BepInEx as in use as well.
        logger.Debug("Flagging BepInEx as in use...");
        loaderType |= LoaderType.BepInEx;

        try {
            logger.Debug("Invoking BepInEx preloader...");
            main.Invoke(null, null);
        }
        catch (Exception e) {
            logger.Error("Failed to invoke BepInEx preloader:", e);
        }
    }

    private static void MoveBepInExFolder() {
        if (!Directory.Exists(BEPINEX_DIRECTORY))
            return;
        
        logger.Info("Found existing BepInEx directory, moving over...");

        var full = Path.GetFullPath(BEPINEX_DIRECTORY);
        var fullSisyphus = Path.GetFullPath(SISYPHUS_DIRECTORY);
        var dir = new DirectoryInfo(full);

        foreach (var file in dir.EnumerateFiles("**", SearchOption.AllDirectories)) {
            var path = file.FullName.Replace(full, fullSisyphus);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            file.MoveTo(path);
        }
        
        dir.Delete(true);
    }
}
