using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using MonoMod.RuntimeDetour.HookGen;
using Sisyphus.Loader.Core;

namespace Sisyphus;

/// <summary>
///     The actual entrypoint for sisyphus, called by other entrypoints.
/// </summary>
/// <seealso cref="Doorstop.Entrypoint"/>
/// 
internal static class Entrypoint {
    internal static void Main(LoaderType loaderType) {
        var modDir = Path.Combine("sisyphus", "sisyphus-mods");
        IModLoader loader = new ModLoader(modDir) {
            LoaderEnvironment = loaderType,
        };

        AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        AppDomain.CurrentDomain.AssemblyLoad += Patch;

        try {
            InitializeLogging();
            PatchAssemblyLoading();

            var log = LogManager.GetLogger("Entrypoint");

            log.Info($"Stating with {nameof(loaderType)}: {loaderType}");
            LoadManager.Load(ref loaderType, loader);
            log.Info($"Ending with {nameof(loaderType)}: " + loaderType);
        }
        finally {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
        }
    }

    private static void Patch(object sender, AssemblyLoadEventArgs args) {
        var name = args.LoadedAssembly.GetName().Name;

        switch (name) {
            case "BepInEx.Preloader":
                Patch_BepInExPreloader(args.LoadedAssembly);
                break;
        }
    }

    private static void Patch_BepInExPreloader(Assembly asm) {
        var preloaderRunner = asm.GetType("BepInEx.Preloader.PreloaderRunner");
        var loadCriticalAssemblies = preloaderRunner.GetMethod(
            "LoadCriticalAssemblies",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        var platformUtils = asm.GetType("BepInEx.Preloader.PlatformUtils");
        var setPlatform = platformUtils.GetMethod(
            "SetPlatform",
            BindingFlags.Public | BindingFlags.Static
        );

        if (loadCriticalAssemblies is not null)
            HookEndpointManager.Add(loadCriticalAssemblies, () => { });

        if (setPlatform is not null)
            HookEndpointManager.Add(setPlatform, () => { });
    }

    private static Assembly? Resolve(object sender, ResolveEventArgs args) {
        var name = new AssemblyName(args.Name);

        try {
            var fileName = name.Name + ".dll";
            var path = Path.Combine("sisyphus", "sisyphus-core", fileName);
            return Assembly.LoadFile(path);
        }
        catch {
            return null;
        }
    }

    private static void InitializeLogging() {
        var layout = new PatternLayout {
            ConversionPattern = LOG_PATTERN,
        };
        layout.ActivateOptions();

        var appenders = new List<IAppender> {
            new ConsoleAppender {
                Name = nameof(ConsoleAppender),
                Layout = layout,
            },
            new DebugAppender {
                Name = nameof(DebugAppender),
                Layout = layout,
            },
        };

        var fileApp = new FileAppender {
            Layout = layout,
            File = "sisyphus-preload.log",
            AppendToFile = false,
            Encoding = Encoding.UTF8,
        };

        fileApp.ActivateOptions();
        appenders.Add(fileApp);

        BasicConfigurator.Configure(appenders.ToArray());
    }

    private static void PatchAssemblyLoading() {
        // AppDomain.CurrentDomain.Load()
    }
}
