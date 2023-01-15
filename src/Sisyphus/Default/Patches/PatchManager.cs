using System.Collections.Generic;
using Sisyphus.Loader.API;

namespace Sisyphus.Default.Patches; 

internal sealed class PatchManager {
    public IEnumerable<IPrepatcher> GetPatchers() {
        yield break;
    }
}
