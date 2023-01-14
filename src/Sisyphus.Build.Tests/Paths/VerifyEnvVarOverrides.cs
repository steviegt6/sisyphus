// using System;
// using Sisyphus.Build.Util;
// 
// namespace Sisyphus.Build.Tests.Paths; 
// 
// /// <summary>
// ///     Verifies that the environment variables defined in
// ///     <see cref="PathResolution"/> properly return expected values instead
// ///     of running through regular logic.
// /// </summary>
// [TestFixture]
// public static class VerifyEnvVarOverrides {
//     [Test]
//     public static void OverrideGamePathTest() {
//         const string key = PathResolution.SISYPHUS_UK_GAME_PATH;
//         const string expected = "expected";
//         var log = Logger.Create(nameof(OverrideGamePathTest));
// 
//         Environment.SetEnvironmentVariable(key, expected);
//         Assert.That(PathResolution.GetGamePath(log), Is.EqualTo(expected));
//     }
//     
//     [Test]
//     public static void OverrideAssemblyPathTest() {
//         const string key = PathResolution.SISYPHUS_UK_ASSEMBLY_PATH;
//         const string expected = "expected";
//         var log = Logger.Create(nameof(OverrideAssemblyPathTest));
// 
//         Environment.SetEnvironmentVariable(key, expected);
//         Assert.That(PathResolution.GetAssemblyPath(log), Is.EqualTo(expected));
//     }
// }
