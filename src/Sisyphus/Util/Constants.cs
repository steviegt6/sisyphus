using Styles = Semver.SemVersionStyles;
using ROptions = Semver.Ranges.SemVersionRangeOptions;

namespace Sisyphus.Util;

/// <summary>
///     Useful constant utility values.
/// </summary>
public static class Constants {
    /// <summary>
    ///     <see cref="Styles"/> that mods are held to.
    /// </summary>
    public const Styles SEMVER_STYLES = Styles.Strict
                                      | Styles.AllowLeadingZeros
                                      | Styles.OptionalPatch
                                      | Styles.OptionalMinorPatch;

    /// <summary>
    ///     <see cref="ROptions"/> that mods are held to.
    /// </summary>
    public const ROptions RANGE_OPTIONS = (ROptions) SEMVER_STYLES
                                        | ROptions.IncludeAllPrerelease
                                        | ROptions.AllowMetadata;
    
    /// <summary>
    ///     Standard log pattern used for log4net logging.
    /// </summary>
    public const string LOG_PATTERN =
        "[%d{HH:mm:ss.fff}] [%t/%level] [%logger]: %m%n";
}
