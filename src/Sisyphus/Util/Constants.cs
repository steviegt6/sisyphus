using Styles = Semver.SemVersionStyles;
using ROptions = Semver.Ranges.SemVersionRangeOptions;

namespace Sisyphus.Util;

/// <summary>
///     Useful constant utility values.
/// </summary>
public static class Constants {
    /// <summary>
    ///     <see cref="Styles"/> that mods are held accountable to.
    /// </summary>
    public const Styles SEMVER_STYLES = Styles.Strict
                                      | Styles.AllowLeadingZeros
                                      | Styles.OptionalPatch
                                      | Styles.OptionalMinorPatch;

    public const ROptions RANGE_OPTIONS = (ROptions) SEMVER_STYLES
                                        | ROptions.IncludeAllPrerelease
                                        | ROptions.AllowMetadata;
}
