using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Semver;
using Semver.Ranges;

namespace Sisyphus.Loader.API;

/// <summary>
///     Describes metadata for a mod.
/// </summary>
public interface IModMetadata {
    /// <summary>
    ///     The mod's display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     SemVer-compliant mod version.
    /// </summary>
    SemVersion Version { get; }

    /// <summary>
    ///     The authors of this mod.
    /// </summary>
    string Authors { get; }

    /// <summary>
    ///     A (preferably) short and sweet, brief rundown of the mod.
    /// </summary>
    string Description { get; }

    Dictionary<string, SemVersionRange> Dependencies { get; }
}

/// <summary>
///     An implementation of <see cref="IModMetadata"/> that may be deserialized
///     from a JSON file.
/// </summary>
public class JsonModMetadata : IModMetadata {
    /*/// <summary>
    ///     Parity with the standard <see cref="ModDependency"/> type, just with
    ///     the version range represented as an unparsed string for convenient
    ///     JSON deserialization.
    /// </summary>
    public class JsonModDependency {
        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("Version")]
        public string Version { get; }
    }*/

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; } = null!;

    [JsonProperty("version", Required = Required.Always)]
    private string version { get; } = null!;

    [JsonIgnore]
    public SemVersion Version => SemVersion.Parse(version, SEMVER_STYLES);

    [JsonProperty("authors", Required = Required.Always)]
    public string Authors { get; } = null!;

    [JsonProperty("description")]
    public string Description { get; } = null!;

    [JsonProperty("dependencies", Required = Required.Always)]
    private Dictionary<string, string> dependencies { get; } = null!;

    [JsonIgnore]
    public Dictionary<string, SemVersionRange> Dependencies {
        get {
            return dependencies.ToDictionary(
                x => x.Key,
                x => SemVersionRange.Parse(x.Value, RANGE_OPTIONS)
            );
        }
    }
}
