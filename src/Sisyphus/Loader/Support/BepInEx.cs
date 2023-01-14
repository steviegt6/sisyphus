using System;
using System.IO;
using System.Reflection;
using System.Threading;
using log4net;

namespace Sisyphus.Loader.Support;

/// <summary>
///     BepInEx support for sisyphus.
/// </summary>
internal static class BepInEx {
    private static readonly ILog logger =
        LogManager.GetLogger("Support.BepInEx");

    private const string sisyphus_dir = "sisyphus";
    private const string bepinex_dir = "BepInEx";
    private const string core_dir = "core";
    private const string bepinex_preloader_dll = "BepInEx.Preloader.dll";

    private const string ep_name = "BepInEx.Preloader.Entrypoint";
    private const string main_name = "Main";

    private static readonly string bepinex_preloader_path =
        Path.Combine(sisyphus_dir, core_dir, bepinex_preloader_dll);

    internal static void Initialize(ref LoaderType loaderType) {
        logger.Info("Initializing BepInEx support...");

        MoveBepInExFolder();

        var expectedDir = Path.Combine(sisyphus_dir, core_dir);

        if (!Directory.Exists(expectedDir)) {
            logger.Info("Skipping, not found: " + expectedDir);
            return;
        }

        logger.Info("Found: " + expectedDir);

        Assembly asm;

        try {
            logger.Debug("Loading assembly: " + bepinex_preloader_path);
            asm = Assembly.LoadFile(bepinex_preloader_path);
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
        if (!Directory.Exists(bepinex_dir))
            return;
        
        logger.Info("Found existing BepInEx directory, moving over...");

        var full = Path.GetFullPath(bepinex_dir);
        var fullSisyphus = Path.GetFullPath(sisyphus_dir);
        var dir = new DirectoryInfo(full);

        foreach (var file in dir.EnumerateFiles("**", SearchOption.AllDirectories)) {
            var path = file.FullName.Replace(full, fullSisyphus);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            file.MoveTo(path);
        }
        
        dir.Delete(true);
    }
}
