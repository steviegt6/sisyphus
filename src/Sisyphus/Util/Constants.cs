using System.IO;
using Styles = Semver.SemVersionStyles;
using ROptions = Semver.Ranges.SemVersionRangeOptions;

namespace Sisyphus.Util;

/// <summary>
///     Useful constant utility values.
/// </summary>
public static class Constants {
#region Miscellaneous
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
#endregion

#region Paths
    public const string SISYPHUS_PRELOAD_LOG_FILE = "sisyphus-preload.log";

    public const string METADATA_FILE = "metadata.json";

    public const string BEPINEX_DIRECTORY = "BepInEx";

    public const string BEPINEX_CORE_DIRECTORY = "core";

    public const string BEPINEX_PRELOADER_FILE = "BepInEx.Preloader.dll";

    public const string SISYPHUS_DIRECTORY = "sisyphus";

    public const string SISYPHUS_CORE_DIRECTORY = "sisyphus-core";

    public const string SISYPHUS_MODS_DIRECTORY = "sisyphus-mods";

    public static readonly string SISYPHUS_BEPINEX_CORE_PATH =
        Path.Combine(SISYPHUS_DIRECTORY, BEPINEX_CORE_DIRECTORY);

    public static readonly string BEPINEX_PRELOADER_PATH =
        Path.Combine(SISYPHUS_BEPINEX_CORE_PATH, BEPINEX_PRELOADER_FILE);

    public static readonly string SISYPHUS_CORE_PATH =
        Path.Combine(SISYPHUS_DIRECTORY, SISYPHUS_CORE_DIRECTORY);

    public static readonly string SISYPHUS_MODS_PATH =
        Path.Combine(SISYPHUS_DIRECTORY, SISYPHUS_MODS_DIRECTORY);
#endregion
}
