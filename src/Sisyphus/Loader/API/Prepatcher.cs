using Mono.Cecil;

namespace Sisyphus.Loader.API; 

/// <summary>
///     A prepatcher which may perform modifications on assemblies as they get
///     loaded.
/// </summary>
public interface IPrepatcher {
    /// <summary>
    ///     A unique identifier for this prepatcher.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Called when an assembly is going to be loaded. This method may
    ///     modify an assembly before it is loaded into the current AppDomain.
    /// </summary>
    /// <param name="module">The module you may modify.</param>
    /// <returns><see langword="true"/> if the module was modified.</returns>
    bool Modify(ModuleDefinition module);
}

/// <summary>
///     Default implementation of <see cref="IPrepatcher"/>.
/// </summary>
public abstract class Prepatcher : IPrepatcher {
    public abstract string Name { get; }

    public abstract bool Modify(ModuleDefinition module);
}
