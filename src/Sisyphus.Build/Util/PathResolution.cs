using System;
using Microsoft.Build.Utilities;

namespace Sisyphus.Build.Util;

/// <summary>
///     Utilities for resolving paths relating to ULTRAKILL and sispyhus.
/// </summary>
public static class PathResolution {
    public const string SISYPHUS_UK_GAME_PATH = "SISYPHUS_UK_GAME_PATH";
    public const string SISYPHUS_UK_ASSEMBLY_PATH = "SISYPHUS_UK_ASSEMBLY_PATH";

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
        return null; // TODO: Implement automatic resolution for game path.
    }

    /// <summary>
    ///     Retrieves the path to the directory containing .NET assemblies of
    ///     ULTRAKILL and adjacent libraries, often within
    ///     <see cref="GetGamePath"/> but optionally not always.
    /// </summary>
    /// <param name="log">The logger to use.</param>
    /// <returns>
    ///     The path, or <see langword="null"/> if no path is resolved and no
    ///     fallback is provided.
    /// </returns>
    public static string? GetAssemblyPath(TaskLoggingHelper log) {
        var path = GetAssemblyPath_Inner(log);
        
        if (path is null) {
            log.LogError("Failed to resolve ULTRAKILL assembly path.");
            return null;
        }
        
        log.LogMessage($"Resolved ULTRAKILL assembly path: {path}");
        return path;
    }
    
    private static string? GetAssemblyPath_Inner(TaskLoggingHelper log) {
        log.LogMessage("Resolving ULTRAKILL assemblies path...");
        log.LogMessage("Checking env. var.: " + SISYPHUS_UK_ASSEMBLY_PATH);
        var path = Environment.GetEnvironmentVariable(SISYPHUS_UK_ASSEMBLY_PATH);
        
        if (path != null) {
            log.LogMessage("Resolved path from env. var.: " + path);
            return path;
        }
        
        log.LogMessage("No path found in env. var., resolving automatically.");
        return null; // TODO: Implement automatic resolution for assembly path.
    }
}
