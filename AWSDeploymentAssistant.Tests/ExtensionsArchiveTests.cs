using System.Collections.Specialized;
using System.IO.Compression;
using AWSDeploymentAssistant;

namespace AWSDeploymentAssistant.Tests;

public class ExtensionsArchiveTests
{
    [Test]
    public void ToArray_ReturnsAllValuesInOrder()
    {
        var source = new StringCollection { "alpha", "beta", "gamma" };

        var result = source.ToArray();

        global::NUnit.Framework.Assert.That(result, Is.EqualTo(new[] { "alpha", "beta", "gamma" }));
    }

    [Test]
    public void AddDirectory_AddsOnlyWhitelistedAndNonExcludedFiles()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "AwsDeployAssistantTests_" + Guid.NewGuid().ToString("N"));
        var sourceDir = Path.Combine(tempRoot, "source");
        Directory.CreateDirectory(sourceDir);
        Directory.CreateDirectory(Path.Combine(sourceDir, "sub"));

        try
        {
            File.WriteAllText(Path.Combine(sourceDir, "keep.txt"), "ok");
            File.WriteAllText(Path.Combine(sourceDir, "skip.log"), "no");
            File.WriteAllText(Path.Combine(sourceDir, "sub", "keep2.txt"), "ok");
            File.WriteAllText(Path.Combine(sourceDir, "sub", "ignored.txt"), "no");

            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                archive.AddDirectory(new DirectoryInfo(sourceDir), new[] { ".txt" }, new[] { "ignored" }, recursive: true);
            }

            ms.Position = 0;
            using var readArchive = new ZipArchive(ms, ZipArchiveMode.Read);
            var names = readArchive.Entries.Select(e => e.FullName.Replace('\\', '/')).OrderBy(x => x).ToArray();

            global::NUnit.Framework.Assert.That(names, Is.EqualTo(new[] { "keep.txt", "sub/keep2.txt" }));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }
}
