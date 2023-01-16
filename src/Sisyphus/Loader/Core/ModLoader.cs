using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Medallion.Collections;
using Mono.Cecil;
using Newtonsoft.Json;
using Sisyphus.Exceptions;
using Sisyphus.Loader.API;

namespace Sisyphus.Loader.Core;

/// <summary>
///     Handles collecting, sorting, and loading mods. Includes versioning and
///     basic dependency management.
/// </summary>
public interface IModLoader {
    /// <summary>
    ///     Exposes an event to mods that lets them receive notifications when
    ///     an assembly gets loaded, useful for applying MonoMod patches, etc.
    ///     while avoiding prepatching in unnecessary cases.
    /// </summary>
    event EventHandler<Assembly?>? AssemblyLoaded;

    /// <summary>
    ///     The environment that the mod loader is running in. That is, what
    ///     other loaders are present.
    /// </summary>
    LoaderType LoaderEnvironment { get; set; }

    /// <summary>
    ///     Returns a list of resolved mods.
    /// </summary>
    /// <returns>A list of all mods this loader resolved.</returns>
    [Pure]
    internal List<ResolvedMod> ResolveMods();

    /// <summary>
    ///     Sorts and validates the given list of mods.
    /// </summary>
    /// <param name="mods">The mods to sort and validate.</param>
    /// <returns>A sorted and validated collection of mods.</returns>
    internal List<ResolvedMod> SortAndValidateMods(List<ResolvedMod> mods);

    /// <summary>
    ///     Loads the given mods.
    /// </summary>
    /// <param name="mods">The mods to load.</param>
    internal List<IMod> LoadMods(List<ResolvedMod> mods);

    internal void RegisterPrepatchers(IEnumerable<IPrepatcher> prepatchers);

    internal Assembly LoadAssemblyFromPath(string path);

    internal void OnAssemblyLoaded(Assembly? asm);
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
    private readonly List<IPrepatcher> patchers = new();

    public ModLoader(string modDir) {
        this.modDir = modDir;
    }

    public event EventHandler<Assembly?>? AssemblyLoaded;

    public LoaderType LoaderEnvironment { get; set; }

    public List<ResolvedMod> ResolveMods() {
        log.Debug("Resolving mods...");
        log.Debug("Searching directory: " + modDir);

        var mods = new List<ResolvedMod>();

        foreach (var dir in new DirectoryInfo(modDir).EnumerateDirectories()) {
            log.Debug("Found directory: " + dir.Name);

            var mdPath = Path.Combine(dir.FullName, METADATA_FILE);
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

    public List<ResolvedMod> SortAndValidateMods(List<ResolvedMod> mods) {
        log.Debug($"Validating {mods.Count} mods...");

        // check for duplicates
        var duplicates = mods.GroupBy(m => m.Name)
                             .Where(g => g.Count() > 1)
                             .Select(g => g.Key)
                             .ToList();

        if (duplicates.Count > 0) {
            var msg = "Duplicate mods found: " + string.Join(", ", duplicates);
            throw new DuplicateModsException(msg);
        }

        // Sort topologically, then secondarily by name (alphabetical order).
        // Another option would be the stable sort (StableOrderTopologicallyBy)
        // algorithm provided by the library, but eh.
        // When encountering circular or invalid dependencies, exception will be
        // thrown. It is expected that the method calling this method (or the
        // method calling said method, etc.) will catch the exception and act
        // accordingly.
        var dict = mods.ToDictionary(x => x.Name, x => x);

        IEnumerable<string> sort(string x) {
            var mod = dict[x];

            return mod.Metadata.Dependencies.Count == 0
                ? Enumerable.Empty<string>()
                : mod.Metadata.Dependencies.Select(y => y.Key);
        }

        var sortedNames = dict.Keys.OrderTopologicallyBy(sort).ThenBy(x => x);

        var sortedMods = new List<ResolvedMod>();

        foreach (var name in sortedNames) {
            var mod = dict[name];

            // Ensure the present mods actually fulfill the dependencies of
            // the mods that depend on them.
            foreach (var dep in mod.Metadata.Dependencies) {
                if (!dict.ContainsKey(dep.Key)) {
                    var msg = $"Mod {mod.Name} depends on {dep.Key}, but it is "
                            + "not present!";
                    throw new KeyNotFoundException(msg);
                }

                var actualVer = dict[dep.Key].Metadata.Version;
                var expectedVer = dep.Value;

                if (!actualVer.Satisfies(expectedVer)) {
                    var msg = $"Mod {mod.Name} depends on {dep.Key} "
                            + $"version {expectedVer}, but {dep.Key} is "
                            + $"version {actualVer}!";
                    throw new KeyNotFoundException(msg);
                }
            }

            sortedMods.Add(mod);
        }

        return sortedMods;
    }

    public List<IMod> LoadMods(List<ResolvedMod> mods) {
        var loadedMods = new List<IMod>();

        foreach (var mod in mods) {
            log.Debug("Loading mod: " + mod.Name);

            var path = Path.Combine(mod.Directory, $"{mod.Name}.dll");
            var asm = Assembly.LoadFile(path);
            IMod? instance = null;

            log.Debug($"Looking for class implementing {nameof(IMod)}...");

            foreach (var type in asm.GetTypes()) {
                if (!type.IsClass || type.IsAbstract)
                    continue;

                var ctor = type.GetConstructor(
                    BindingFlags.Public
                  | BindingFlags.NonPublic
                  | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                );
                if (ctor is null)
                    continue;

                if (type.GetInterface(nameof(IMod)) is null)
                    continue;

                // Use the ctor instead of Activator.CreateInstance since we
                // already go through the trouble of getting the ctor for
                // precondition checks above.
                instance = (IMod) ctor.Invoke(null);
                instance.Metadata = mod.Metadata;
                instance.Loader = this;
                instance.OnInitialize();
                RegisterPrepatchers(instance.GetPrepatchers());

                loadedMods.Add(instance);

                // We have found we we came for.
                break;
            }

            if (instance is null) {
                // TODO: What to do here? Should we throw?
                log.Warn($"No class implementing {nameof(IMod)} found!");
            }
        }

        return loadedMods;
    }

    public void RegisterPrepatchers(IEnumerable<IPrepatcher> prepatchers) {
        patchers.AddRange(prepatchers);
    }

    public Assembly LoadAssemblyFromPath(string path) {
        var module = ModuleDefinition.ReadModule(path);

        bool modified = false;

        foreach (var patcher in patchers) {
            log.Debug($"Patching {module.Name} with {patcher.Name}...");

            var patched = patcher.Modify(module);
            modified |= patched;

            if (patched)
                log.Debug($"Patched {module.Name} with {patcher.Name}!");
            else
                log.Debug($"{patcher.Name} made no alleged changes!");
        }

        if (!modified) {
            log.Debug($"No reported changes made to ${module.Name}, skipping!");
            return Assembly.LoadFrom(path);
        }

        using var ms = new MemoryStream();
        module.Write(ms);
        ms.Seek(0, SeekOrigin.Begin);

        return Assembly.Load(ms.ToArray());
    }

    public void OnAssemblyLoaded(Assembly? asm) {
        AssemblyLoaded?.Invoke(this, asm);
    }
}
