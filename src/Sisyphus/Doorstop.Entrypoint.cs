using Sisyphus.Loader;
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
        Sisyphus.Entrypoint.Main(LoaderType.UnityDoorstop);
    }
}
