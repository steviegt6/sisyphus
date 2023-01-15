using System.Collections.Generic;
using Semver;
using Semver.Ranges;

namespace Sisyphus.Loader.API; 

/// <summary>
///     Represents the core component of a mod that uses sisyphus.
/// </summary>
public interface IMod {
    /// <summary>
    ///     This mod's metadata.
    /// </summary>
    IModMetadata? Metadata { get; set; }
}

/// <summary>
///     The default implementation of <see cref="IMod"/>.
/// </summary>
public abstract class Mod : IMod {
    // It can be safely assumed that this property will be properly set before
    // a mod actually tries to do anything with the metadata, but it should
    // still be considered nullable to avoid any edge cases.
    public IModMetadata? Metadata { get; set; } = null;
}
