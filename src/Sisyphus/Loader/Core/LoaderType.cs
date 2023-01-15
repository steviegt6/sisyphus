using System;

namespace Sisyphus.Loader.Core;

/// <summary>
///     Describes known mod loaders the user is using.
/// </summary>
[Flags]
internal enum LoaderType {
    /// <summary>
    ///     UnityDoorstop is in use. Generally should always be true.
    ///     <br />
    ///     https://github.com/NeighTools/UnityDoorstop
    /// </summary>
    UnityDoorstop = 0b0001,
    
    /// <summary>
    ///     BepInEx is in use.
    ///     <br />
    ///     https://github.com/BepInEx/BepInEx
    /// </summary>
    BepInEx       = 0b0010,
    
    /// <summary>
    ///     MelonLoader is in use.
    ///     <br />
    ///     https://github.com/LavaGang/MelonLoader
    /// </summary>
    MelonLoader   = 0b0100,
    
    /// <summary>
    ///     UltraModManager is in use.
    ///     <br />
    ///     https://github.com/Temperz87/ultra-mod-manager/
    /// </summary>
    UMM           = 0b1000,
}
