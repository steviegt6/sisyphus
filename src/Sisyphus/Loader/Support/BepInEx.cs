using System;
using System.IO;
using System.Reflection;
using log4net;

namespace Sisyphus.Loader.Support;

/// <summary>
///     BepInEx support for sisyphus.
/// </summary>
internal static class BepInEx {
    private static readonly ILog logger =
        LogManager.GetLogger("Support.BepInEx");

    private const string bepinex_dir = "BepInEx";
    private const string core_dir = "core";
    private const string bepinex_preloader_dll = "BepInEx.Preloader.dll";

    private const string ep_name = "Entrypoint";
    private const string main_name = "Main";

    private static readonly string bepinex_preloader_path =
        Path.Combine(bepinex_dir, core_dir, bepinex_preloader_dll);

    internal static void Initialize(ref LoaderType loaderType) {
        logger.Info("Initializing BepInEx support...");

        var expectedDir = Path.Combine(bepinex_dir, core_dir);

        if (!Directory.Exists(expectedDir)) {
            logger.Info("Skipping, not found: " + expectedDir);
            return;
        }

        logger.Info("Found: " + expectedDir);

        var setup = new AppDomainSetup {
            ApplicationBase = expectedDir,
        };
        AppDomain domain;

        try {
            logger.Debug("Initializing AppDomain...");
            domain = AppDomain.CreateDomain("Sisyphus:BepInEx", null, setup);
        }
        catch (Exception e) {
            logger.Error("Failed to initialize AppDomain:", e);
            return;
        }

        Assembly asm;

        try {
            logger.Debug("Loading assembly: " + bepinex_preloader_path);
            asm = domain.Load(File.ReadAllBytes(bepinex_preloader_path));
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
            BindingFlags.Static | BindingFlags.NonPublic
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
}
