// This is used for personal testing and should not run during any automated
// test runs or the like.

#if true
using Sisyphus.Build.Util;

namespace Sisyphus.Build.Tests.Paths;

[TestFixture]
public static class VerifyAutoPathResolution {
    [Test]
    public static void AutoGamePathTest() {
        var log = Logger.Create(nameof(AutoGamePathTest));

        Assert.That(PathResolution.GetGamePath(log), Is.Not.Null);
    }

    [Test]
    public static void AutoAssemblyPathTest() {
        var log = Logger.Create(nameof(AutoAssemblyPathTest));

        var gamePath = PathResolution.GetGamePath(log);
        if (gamePath is null)
            Assert.Fail("Failed to get game path before assembly path.");

        Assert.That(PathResolution.GetAssemblyPath(log, gamePath), Is.Not.Null);
    }
}
#endif
