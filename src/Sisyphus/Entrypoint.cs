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
using Sisyphus.Loader;

namespace Sisyphus;

/// <summary>
///     The actual entrypoint for sisyphus, called by other entrypoints.
/// </summary>
/// <seealso cref="Doorstop.Entrypoint"/>
/// 
internal static class Entrypoint {
    private const string log_pattern =
        "[%d{HH:mm:ss.fff}] [%t/%level] [%logger]: %m%n";

    internal static void Main(LoaderType loaderType) {
        AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        AppDomain.CurrentDomain.UnhandledException += Handle;
        AppDomain.CurrentDomain.AssemblyLoad += Patch;

        try {
            InitializeLogging();

            var log = LogManager.GetLogger("Entrypoint");

            log.Info($"Stating with {nameof(loaderType)}: {loaderType}");

            log.Info($"{nameof(LoadManager)}::{nameof(LoadManager.Load)}");

            LoadManager.Load(ref loaderType);

            log.Info("Detected loaders: " + loaderType);
        }
        finally {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
            AppDomain.CurrentDomain.UnhandledException -= Handle;
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

    private static void Handle(object sender, UnhandledExceptionEventArgs e) {
        if (e.ExceptionObject is not Exception ex)
            return;

        var log = LogManager.GetLogger("UnhandledExceptionHandler");
        log.Fatal("Unhandled exception:", ex);
    }

    private static Assembly? Resolve(object sender, ResolveEventArgs args) {
        var name = new AssemblyName(args.Name);

        try {
            var path =
                Path.Combine("sisyphus", "sisyphus-core", name.Name + ".dll");
            return Assembly.LoadFile(path);
        }
        catch {
            return null;
        }
    }

    private static void InitializeLogging() {
        var layout = new PatternLayout {
            ConversionPattern = log_pattern
        };
        layout.ActivateOptions();

        var appenders = new List<IAppender> {
            new ConsoleAppender {
                Name = "ConsoleAppender",
                Layout = layout,
            },
            new DebugAppender {
                Name = "DebugAppender",
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

        LogManager.GetLogger("InitializeLogging").Info("Initialized logging!");
    }
}
