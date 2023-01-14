using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Framework;

namespace Sisyphus.Build;

/// <summary>
///     Collects *.dll files from a given path to load.
/// </summary>
[UsedImplicitly]
public sealed class AssemblyResolutionTask : BuildTask {
    [Required]
    public string InputPath { get; [UsedImplicitly] set; } = "";

    [Output]
    public string[] Assemblies { [UsedImplicitly] get; set; } = Array.Empty<string>();

    public override bool Execute() {
        if (!Directory.Exists(InputPath)) {
            Log.LogError("Directory not found: " + InputPath);
            return false;
        }

        Log.LogMessage($"Collecting assemblies from {InputPath}...");

        var assemblies = Directory.EnumerateFiles(InputPath, "*.dll");

        Assemblies = assemblies.Where(Filter).ToArray();

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
