using Sisyphus.Loader;

// ReSharper disable once CheckNamespace
namespace Doorstop;

/// <summary>
///     UnityDoorstop entry-point.
/// </summary>
[UsedImplicitly]
public static class Entrypoint {
    /// <summary>
    ///     Entrypoint method invoked externally by UnityDoorstop.
    /// </summary>
    [UsedImplicitly]
    public static void Start() {
        Sisyphus.Entrypoint.Main(LoaderType.UnityDoorstop);
    }
}
