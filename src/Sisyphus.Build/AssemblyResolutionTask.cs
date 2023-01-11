using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Sisyphus.Build.Util;

namespace Sisyphus.Build;

/// <summary>
///     Collects *.dll files from a given path to load.
/// </summary>
public sealed class AssemblyResolutionTask : BuildTask {
    //[Required]
    public string? InputPath { get; set; } = "";

    [Output]
    public string[] Assemblies { get; set; } = Array.Empty<string>();

    public override bool Execute() {
        // TODO: Temporary workaround.
        var gamePath = PathResolution.GetGamePath(Log);
        InputPath = PathResolution.GetAssemblyPath(Log, gamePath);

        if (InputPath is null) {
            Log.LogError("Input path is null.");
            return false;
        }

        Log.LogMessage($"Collecting assemblies from {InputPath}...");

        var assemblies = Directory.EnumerateFiles(InputPath, "*.dll");

        Assemblies = assemblies.Where(x => Filter(x)).ToArray();

        Log.LogMessage($"Found {Assemblies.Length} assemblies:");

        foreach (var assembly in Assemblies) {
            Log.LogMessage($"  {assembly}");
        }

        return true;
    }

    private static bool Filter(string assembly) {
        return !assembly.EndsWith("System.dll")
            && !assembly.EndsWith("System.Core.dll");
    }
}
