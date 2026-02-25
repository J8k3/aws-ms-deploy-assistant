using System.Reflection;
using AWSDeploymentAssistant;

namespace CodeDeployPlugin.Tests;

public class AWSCodeDeployAppSpecGeneratorTests
{
    [Test]
    public void MetadataProperties_ReturnExpectedValues()
    {
        var generator = new AWSCodeDeployAppSpecGenerator();

        global::NUnit.Framework.Assert.That(generator.Name, Is.EqualTo("AWSCodeDeployPlugin"));
        global::NUnit.Framework.Assert.That(generator.LoadOptions, Is.True);
        global::NUnit.Framework.Assert.That(generator.ThrowOnError, Is.True);
        global::NUnit.Framework.Assert.That(generator.Priority, Is.EqualTo(9999));
    }

    [Test]
    public void BuildAppSpec_WithDefaults_CreatesDefaultScriptsAndHooks()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "CodeDeployPluginTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var generator = new AWSCodeDeployAppSpecGenerator();
            var appSpec = InvokeBuildAppSpec(generator, new DirectoryInfo(tempRoot));

            global::NUnit.Framework.Assert.That(appSpec.OS, Is.EqualTo("windows"));

            var fileTask = appSpec.Files.Single();
            global::NUnit.Framework.Assert.That(fileTask.Source, Is.EqualTo("\\"));
            global::NUnit.Framework.Assert.That(fileTask.Destination, Is.EqualTo("c:\\inetpub\\wwwroot"));

            global::NUnit.Framework.Assert.That(appSpec.Hooks.ApplicationStop.Single().Location, Is.EqualTo("defaultApplicationStop.ps1"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.BeforeInstall.Single().Location, Is.EqualTo("defaultBeforeInstall.ps1"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.ApplicationStart.Single().Location, Is.EqualTo("defaultApplicationStart.ps1"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.AfterInstall, Is.Null);
            global::NUnit.Framework.Assert.That(appSpec.Hooks.ValidateService, Is.Null);

            var beforeInstallScript = Path.Combine(tempRoot, "defaultBeforeInstall.ps1");
            global::NUnit.Framework.Assert.That(File.Exists(Path.Combine(tempRoot, "defaultApplicationStop.ps1")), Is.True);
            global::NUnit.Framework.Assert.That(File.Exists(beforeInstallScript), Is.True);
            global::NUnit.Framework.Assert.That(File.Exists(Path.Combine(tempRoot, "defaultApplicationStart.ps1")), Is.True);

            var scriptText = File.ReadAllText(beforeInstallScript);
            global::NUnit.Framework.Assert.That(scriptText, Does.Contain("Remove-Item"));
            global::NUnit.Framework.Assert.That(scriptText, Does.Contain("c:\\inetpub\\wwwroot\\*"));
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
    public void BuildAppSpec_WithCustomOptions_UsesCustomHooksAndSkipsDefaultScripts()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "CodeDeployPluginTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var generator = new AWSCodeDeployAppSpecGenerator();
            generator.Options["os"] = "linux";
            generator.Options["destination"] = "/var/www/app";
            generator.Options["applicationstop"] = "scripts/stop.sh";
            generator.Options["beforeinstall"] = "scripts/before.sh";
            generator.Options["afterinstall"] = "scripts/after.sh";
            generator.Options["applicationstart"] = "scripts/start.sh";
            generator.Options["validateservice"] = "scripts/validate.sh";

            var appSpec = InvokeBuildAppSpec(generator, new DirectoryInfo(tempRoot));

            global::NUnit.Framework.Assert.That(appSpec.OS, Is.EqualTo("linux"));
            global::NUnit.Framework.Assert.That(appSpec.Files.Single().Destination, Is.EqualTo("/var/www/app"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.ApplicationStop.Single().Location, Is.EqualTo("scripts/stop.sh"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.BeforeInstall.Single().Location, Is.EqualTo("scripts/before.sh"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.AfterInstall!.Single().Location, Is.EqualTo("scripts/after.sh"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.ApplicationStart.Single().Location, Is.EqualTo("scripts/start.sh"));
            global::NUnit.Framework.Assert.That(appSpec.Hooks.ValidateService!.Single().Location, Is.EqualTo("scripts/validate.sh"));

            global::NUnit.Framework.Assert.That(File.Exists(Path.Combine(tempRoot, "defaultApplicationStop.ps1")), Is.False);
            global::NUnit.Framework.Assert.That(File.Exists(Path.Combine(tempRoot, "defaultBeforeInstall.ps1")), Is.False);
            global::NUnit.Framework.Assert.That(File.Exists(Path.Combine(tempRoot, "defaultApplicationStart.ps1")), Is.False);
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
    public void BuildAppSpec_WithInvalidOs_ThrowsArgumentException()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "CodeDeployPluginTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var generator = new AWSCodeDeployAppSpecGenerator();
            generator.Options["os"] = "macos";

            global::NUnit.Framework.Assert.Throws<ArgumentException>(() => InvokeBuildAppSpec(generator, new DirectoryInfo(tempRoot)));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }

    private static AppSpec InvokeBuildAppSpec(AWSCodeDeployAppSpecGenerator generator, DirectoryInfo workingDirectory)
    {
        var method = typeof(AWSCodeDeployAppSpecGenerator).GetMethod("BuildAppSpec", BindingFlags.Instance | BindingFlags.NonPublic);
        global::NUnit.Framework.Assert.That(method, Is.Not.Null, "BuildAppSpec method was not found via reflection.");

        try
        {
            return (AppSpec)method!.Invoke(generator, new object?[] { new BuildRequest(), workingDirectory })!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }
}
