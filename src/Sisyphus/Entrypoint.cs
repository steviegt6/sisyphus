using Sisyphus.Loader;

namespace Sisyphus;

/// <summary>
///     The actual entrypoint for sisyphus, called by other entrypoints.
/// </summary>
/// <seealso cref="Doorstop.Entrypoint"/>
/// 
internal static class Entrypoint {
    internal static void Main(LoaderType loaderType) {
        // File.WriteAllText("test.txt", loaderType.ToString());

        LoadManager.Load(ref loaderType);
    }
}
