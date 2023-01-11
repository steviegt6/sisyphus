using Microsoft.Build.Framework;
using Sisyphus.Build.Util;

namespace Sisyphus.Build;

/// <summary>
///     Resolves relevant ULTRAKILL paths.
/// </summary>
public sealed class PathResolutionTask : BuildTask {
    [Output]
    public string GamePath { get; set; } = "";

    [Output]
    public string AssemblyPath { get; set; } = "";

    public override bool Execute() {
        var gamePath = PathResolution.GetGamePath(Log);
        var assemblyPath = PathResolution.GetAssemblyPath(Log, gamePath);

        var errors = false;

        if (gamePath is null) {
            errors = true;
            Log.LogError("Failed to resolve ULTRAKILL game path.");
        }

        if (assemblyPath is null) {
            errors = true;
            Log.LogError("Failed to resolve ULTRAKILL assembly path.");
        }

        return !errors;
    }
}
