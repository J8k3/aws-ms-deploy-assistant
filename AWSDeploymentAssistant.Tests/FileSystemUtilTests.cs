using System.Text;
using AWSDeploymentAssistant;

namespace AWSDeploymentAssistant.Tests;

public class FileSystemUtilTests
{
    [Test]
    public void WriteFile_WithNoAppend_WritesProvidedBytes()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "AwsDeployAssistantTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var filePath = Path.Combine(tempRoot, "output.txt");

        try
        {
            FileSystemUtil.WriteFile(filePath, Encoding.UTF8.GetBytes("Header"));

            var text = File.ReadAllText(filePath);

            global::NUnit.Framework.Assert.That(text, Is.EqualTo("Header"));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }

    [Test]
    public void WriteFile_WithAppend_WritesAppendedLines()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "AwsDeployAssistantTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var filePath = Path.Combine(tempRoot, "output.txt");

        try
        {
            FileSystemUtil.WriteFile(filePath, Encoding.UTF8.GetBytes("Header"), "Line1", "Line2");

            var text = File.ReadAllText(filePath);

            global::NUnit.Framework.Assert.That(text, Does.Contain("Line1"));
            global::NUnit.Framework.Assert.That(text, Does.Contain("Line2"));
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
