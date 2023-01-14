using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
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

        try {
            InitializeLogging();

            // HookMM();

            var log = LogManager.GetLogger("Entrypoint");

            log.Info($"Stating with {nameof(loaderType)}: {loaderType}");

            log.Info($"{nameof(LoadManager)}::{nameof(LoadManager.Load)}");

            LoadManager.Load(ref loaderType);
        }
        finally {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
            AppDomain.CurrentDomain.UnhandledException -= Handle;
        }
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
            var path = Path.Combine("sisyphus", "core", name.Name + ".dll");
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

    /*private static void HookMM() {
        var log = LogManager.GetLogger("HookMM");

        log.Info("Performing initial MonoMod hooks...");

        HookMM_IsTransparentProxy(log);
        HookMM_Marshal(log);

        log.Info("Finished initial MonoMod hooks!");
    }

    private static void HookMM_Marshal(ILog log) {
        var type = typeof(RemotingServices);
        var meth = type.GetMethod(
            nameof(RemotingServices.Marshal),
            BindingFlags.Public | BindingFlags.Static,
            null,
            new[] {
                typeof(MarshalByRefObject),
                typeof(string),
                typeof(Type)
            },
            null
        );
        
        if (meth is null) {
            log.Warn("Failed to find RemotingServices::Marshal!");
            return;
        }

        HookEndpointManager.Modify(meth, HookMM_Marshal_Modify);
        HookEndpointManager.Add(meth,
                                (MarshalByRefObject _, string _, Type _) => {
                                    LogManager.GetLogger("test2").Info("ugh");
                                    return (ObjRef?) null;
                                });
    }

    private static void HookMM_Marshal_Modify(ILContext il) {
        var c = new ILCursor(il);

        var type = typeof(RemotingServices);
        const string name = "IsTransparentProxy";

        while (c.TryGotoNext(MoveType.Before, x => x.MatchCall(type, name))) {
            c.Remove(); // remove callvirt
            c.EmitDelegate<Func<object, bool>>(x => {
                var log = LogManager.GetLogger("test");
                log.Info("HI");
                return false;
            }); // assume false
        }
    }

    private static void HookMM_IsTransparentProxy(ILog log) {
        var type = typeof(RemotingServices);
        var meth = type.GetMethod(
            nameof(RemotingServices.IsTransparentProxy),
            BindingFlags.Public | BindingFlags.Static
        );

        if (meth is null) {
            log.Warn("Failed to find RemotingServices::IsTransparentProxy!");
            return;
        }

        // Remoting is not enabled so we should assume false.
        // Normally this throws an error, preventing us from creating
        // AppDomains.
        HookEndpointManager.Add(meth, (object _) => false);
    }*/
}
