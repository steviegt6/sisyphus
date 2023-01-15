using System.Collections.Generic;
using Sisyphus.Loader.Core;

namespace Sisyphus.Loader.API; 

/// <summary>
///     Represents the core component of a mod that uses sisyphus.
/// </summary>
public interface IMod {
    /// <summary>
    ///     This mod's metadata.
    /// </summary>
    IModMetadata? Metadata { get; set; }

    /// <summary>
    ///     The <see cref="IModLoader"/> that loaded this <see cref="IMod"/>.
    /// </summary>
    IModLoader? Loader { get; set; }

    /// <summary>
    ///     Called once this mod is initialized and <see cref="Metadata"/> has
    ///     been set.
    /// </summary>
    void OnInitialize();

    /// <summary>
    ///     Called once sisyphus has finished loading its mods and supported
    ///     loaders have been initialized.
    /// </summary>
    void OnLoadFinish();

    /// <summary>
    ///     Retrieves a list of prepatchers that this mod provides and employs.
    /// </summary>
    /// <returns>An enumerable collection of prepatchers to be used.</returns>
    IEnumerable<IPrepatcher> GetPrepatchers();
}

/// <summary>
///     The default implementation of <see cref="IMod"/>.
/// </summary>
public abstract class Mod : IMod {
    // It can be safely assumed that this property will be properly set before
    // a mod actually tries to do anything with the metadata, but it should
    // still be considered nullable to avoid any edge cases.
    public IModMetadata? Metadata { get; set; } = null;

    // See the above comments about Metadata and nullability. The same logic
    // applies here.
    public IModLoader? Loader { get; set; }

    public virtual void OnInitialize() {
    }

    public virtual void OnLoadFinish() {
    }

    public virtual IEnumerable<IPrepatcher> GetPrepatchers() {
        yield break;
    }
}
