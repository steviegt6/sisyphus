using Semver;

namespace Sisyphus.Loader.API; 

/// <summary>
///     Describes metadata for a mod.
/// </summary>
public interface IModMetadata {
    /// <summary>
    ///     The name of the mod.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     SemVer-compliant mod version.
    /// </summary>
    SemVersion Version { get; }

    /// <summary>
    ///     The mod's display name.
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    ///     The authors of this mod.
    /// </summary>
    string Authors { get; }

    /// <summary>
    ///     A (preferably) short and sweet, brief rundown of the mod.
    /// </summary>
    string Description { get; }
}
