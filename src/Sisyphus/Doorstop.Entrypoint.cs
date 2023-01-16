using Sisyphus.Loader.Core;

// ReSharper disable once CheckNamespace
namespace Doorstop;

/// <summary>
///     UnityDoorstop entry-point.
/// </summary>
[UsedImplicitly]
internal static class Entrypoint {
    /// <summary>
    ///     Entrypoint method invoked externally by UnityDoorstop.
    /// </summary>
    [UsedImplicitly]
    internal static void Start() {
        Doorstop(LoaderType.UnityDoorstop3);
    }

    [UsedImplicitly]
    internal static void Main() {
        Doorstop(LoaderType.UnityDoorstop4);
    }

    internal static void Doorstop(LoaderType doorstopVers) {
            Sisyphus.Entrypoint.Main(LoaderType.UnityDoorstop | doorstopVers);
    }
}
