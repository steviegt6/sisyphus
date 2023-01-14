namespace Sisyphus.Loader.API; 

/// <summary>
///     Represents the core component of a mod that uses sisyphus.
/// </summary>
public interface IMod {
    /// <summary>
    ///     This mod's metadata.
    /// </summary>
    IModMetadata Metadata { get; }
}
