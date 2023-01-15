using System.Collections.Generic;
using Sisyphus.Default.Patches;
using Sisyphus.Loader.API;

namespace Sisyphus.Default; 

/// <summary>
///     The core sisyphus mod that makes use of sisyphus' API for modifying
///     other assemblies for compatibility, including that of other mods. This
///     mod is guaranteed to load first.
/// </summary>
public sealed class SisyphusMod : Mod {
    private readonly PatchManager patchManager = new();
    
    public override IEnumerable<IPrepatcher> GetPrepatchers() {
        return patchManager.GetPatchers();
    }
}
