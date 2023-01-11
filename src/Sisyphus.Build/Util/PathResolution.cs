using System;
using System.IO;
using Microsoft.Build.Utilities;

namespace Sisyphus.Build.Util;

/// <summary>
///     Utilities for resolving paths relating to ULTRAKILL and sispyhus.
/// </summary>
public static class PathResolution {
    public const string SISYPHUS_UK_GAME_PATH = "SISYPHUS_UK_GAME_PATH";
    public const string SISYPHUS_UK_ASSEMBLY_PATH = "SISYPHUS_UK_ASSEMBLY_PATH";

    private static readonly string home =
        Environment.GetEnvironmentVariable("HOME")!;

    private static string[] possiblePathsWindows = {
        "C:\\Program Files (x86)\\Steam\\steamapps\\common\\ULTRAKILL",
        "C:\\Program Files\\Steam\\steamapps\\common\\ULTRAKILL",
    };

    private static string[] possiblePathsUnix = {
        // linux
        $"{home}/.steam/steam/steamapps/common/ULTRAKILL",
        $"{home}/.local/share/Steam/steamapps/common/ULTRAKILL",
        $"{home}/.var/app/com.valvesoftware.Steam/data/Steam/steamapps/common/ULTRAKILL",

        // mac
        "/Applications/ULTRAKILL.app/Contents/MacOS",
        $"{home}/Library/Application Support/Steam/steamapps/common/ULTRAKILL/Contents/MacOS",
    };

    private static string[] innerAssemblyPaths = {
        Path.Combine("ULTRAKILL_Data", "Managed"),
    };

    /// <summary>
    ///     Retrieves the path to the directory containing an installation of
    ///     ULTRAKILL.
    /// </summary>
    /// <param name="log">The logger to use.</param>
    /// <returns>
    ///     The path, or <see langword="null"/> if no path is resolved and no
    ///     fallback is provided.
    /// </returns>
    public static string? GetGamePath(TaskLoggingHelper log) {
        var path = GetGamePath_Inner(log);

        if (path is null) {
            log.LogError("Failed to resolve ULTRAKILL game path.");
            return null;
        }

        log.LogMessage($"Resolved ULTRAKILL game path: {path}");
        return path;
    }

    private static string? GetGamePath_Inner(TaskLoggingHelper log) {
        log.LogMessage("Resolving ULTRAKILL game path...");
        log.LogMessage("Checking env. var.: " + SISYPHUS_UK_GAME_PATH);
        var path = Environment.GetEnvironmentVariable(SISYPHUS_UK_GAME_PATH);

        if (path != null) {
            log.LogMessage("Resolved path from env. var.: " + path);
            return path;
        }

        log.LogMessage("No path found in env. var., resolving automatically.");

        switch (Environment.OSVersion.Platform) {
            case PlatformID.MacOSX:
            case PlatformID.Unix:
                foreach (var pPath in possiblePathsUnix)
                    if (Directory.Exists(pPath))
                        return pPath;

                break;

            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
                foreach (var pPath in possiblePathsWindows)
                    if (Directory.Exists(pPath))
                        return pPath;

                break;

            case PlatformID.WinCE:
            case PlatformID.Xbox:
            default:
                throw new PlatformNotSupportedException();
        }

        return null;
    }

    /// <summary>
    ///     Retrieves the path to the directory containing .NET assemblies of
    ///     ULTRAKILL and adjacent libraries, often within
    ///     <see cref="GetGamePath"/> but optionally not always.
    /// </summary>
    /// <param name="log">The logger to use.</param>
    /// <param name="gamePath">
    ///     The game path to search within automatically.
    /// </param>
    /// <returns>
    ///     The path, or <see langword="null"/> if no path is resolved and no
    ///     fallback is provided.
    /// </returns>
    public static string? GetAssemblyPath(
        TaskLoggingHelper log,
        string? gamePath = null
    ) {
        var path = GetAssemblyPath_Inner(log, gamePath);

        if (path is null) {
            log.LogError("Failed to resolve ULTRAKILL assembly path.");
            return null;
        }

        log.LogMessage($"Resolved ULTRAKILL assembly path: {path}");
        return path;
    }

    private static string? GetAssemblyPath_Inner(
        TaskLoggingHelper log,
        string? gamePath
    ) {
        log.LogMessage("Resolving ULTRAKILL assemblies path...");
        log.LogMessage("Checking env. var.: " + SISYPHUS_UK_ASSEMBLY_PATH);
        var path =
            Environment.GetEnvironmentVariable(SISYPHUS_UK_ASSEMBLY_PATH);

        if (path != null) {
            log.LogMessage("Resolved path from env. var.: " + path);
            return path;
        }

        if (gamePath is null) {
            log.LogMessage("No game path, cannot resolve assembly path.");
            return null;
        }

        log.LogMessage("No path found in env. var., resolving automatically.");

        foreach (var pPath in innerAssemblyPaths) {
            var fPath = Path.Combine(gamePath, pPath);
            if (Directory.Exists(fPath))
                return fPath;
        }

        return null; // TODO: Implement automatic resolution for assembly path.
    }
}
