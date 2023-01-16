using System;

namespace Sisyphus.Loader.Core;

/// <summary>
///     Describes known mod loaders the user is using.
/// </summary>
[Flags]
public enum LoaderType {
    /// <summary>
    ///     UnityDoorstop is in use. Generally should always be true.
    ///     <br />
    ///     https://github.com/NeighTools/UnityDoorstop
    /// </summary>
    /// <remarks>
    ///     This is present regardless of the Doorstop version.
    /// </remarks>
    /// <seealso cref="UnityDoorstop4"/>
    /// <seealso cref="UnityDoorstop3"/>
    UnityDoorstop   = 0b0000_0001,
    
    /// <summary>
    ///     BepInEx is in use.
    ///     <br />
    ///     https://github.com/BepInEx/BepInEx
    /// </summary>
    BepInEx         = 0b0000_0010,
    
    /// <summary>
    ///     MelonLoader is in use.
    ///     <br />
    ///     https://github.com/LavaGang/MelonLoader
    /// </summary>
    MelonLoader     = 0b0000_0100,
    
    /// <summary>
    ///     UltraModManager is in use.
    ///     <br />
    ///     https://github.com/Temperz87/ultra-mod-manager/
    /// </summary>
    UltraModManager = 0b0000_1000,
    
    /// <summary>
    ///     UnityDoorstop v3 is in use.
    /// </summary>
    UnityDoorstop3  = 0b0001_0000,
    
    /// <summary>
    ///     UnityDoorstop v4 is in use.
    /// </summary>
    UnityDoorstop4  = 0b0010_0000,
}
