using System.Collections.Generic;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Sisyphus.Loader.API;

namespace Sisyphus.Loader.Core; 

/// <summary>
///     Handles collecting, sorting, and loading mods. Includes versioning and
///     basic dependency management.
/// </summary>
internal interface IModLoader {
    /// <summary>
    ///     Returns a list of resolved mods.
    /// </summary>
    /// <returns>A list of all mods this loader resolved.</returns>
    [Pure]
    List<ResolvedMod> ResolveMods();
}

/// <summary>
///     Describes the basic information of a mod.
/// </summary>
internal readonly record struct ResolvedMod(
    string Name,
    string Directory,
    IModMetadata Metadata
);

/// <summary>
///     Default <see cref="IModLoader"/> implementation.
/// </summary>
internal sealed class ModLoader : IModLoader {
    private readonly ILog log = LogManager.GetLogger("ModLoader");
    private readonly string modDir;

    public ModLoader(string modDir) {
        this.modDir = modDir;
    }
    
    public List<ResolvedMod> ResolveMods() {
        log.Debug("Resolving mods...");
        log.Debug("Searching directory: " + modDir);

        var mods = new List<ResolvedMod>();

        foreach (var dir in new DirectoryInfo(modDir).EnumerateDirectories()) {
            log.Debug("Found directory: " + dir.Name);

            var mdPath = Path.Combine(dir.FullName, "metadata.json");
            var asmPath = Path.Combine(dir.FullName, $"{dir.Name}.dll");

            if (!File.Exists(mdPath)) {
                log.Warn($"{mdPath} not found, skipping!");
                continue;
            }

            if (!File.Exists(asmPath)) {
                log.Warn($"{asmPath} not found, skipping!");
                continue;
            }

            var mdText = File.ReadAllText(mdPath);
            var md = JsonConvert.DeserializeObject<JsonModMetadata>(mdText);
            
            mods.Add(new ResolvedMod(dir.Name, dir.FullName, md));
        }
        
        log.Debug("Finished resolving mods, got " + mods.Count + " mods:");

        foreach (var mod in mods) {
            var metadata = mod.Metadata;
            log.Debug($"    {mod.Name} (dir: {mod.Directory})");
            log.Debug($"        name:         {metadata.Name}");
            log.Debug($"        authors:      {metadata.Authors}");
            log.Debug($"        version:      {metadata.Version}");
            log.Debug($"        description:  {metadata.Description}");
            log.Debug($"        dependencies: {metadata.Dependencies.Count}");

            foreach (var dep in metadata.Dependencies)
                log.Debug($"            ${dep.Key} {dep.Value}");
        }

        return mods;
    }
}
