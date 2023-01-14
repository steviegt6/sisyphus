using Sisyphus.Loader;

// ReSharper disable once CheckNamespace
namespace Doorstop;

/// <summary>
///     UnityDoorstop entry-point.
/// </summary>
public static class Entrypoint {
    /// <summary>
    ///     Entrypoint method invoked externally by UnityDoorstop.
    /// </summary>
    public static void Start() {
        Sisyphus.Entrypoint.Main(LoaderType.UnityDoorstop);
    }
}
