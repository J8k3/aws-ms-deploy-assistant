namespace CodeDeployPlugin.Tests;

public class AppSpecModelTests
{
    [Test]
    public void HookEvent_DefaultTimeout_IsConfiguredValue()
    {
        var hook = new HookEvent();

        global::NUnit.Framework.Assert.That(hook.Timeout, Is.EqualTo(3600));
    }

    [Test]
    public void FileSection_Add_AddsExpectedTask()
    {
        var files = new FileSection();

        files.Add("\\", "c:\\deploy");

        var task = files.Single();
        global::NUnit.Framework.Assert.That(task.Source, Is.EqualTo("\\"));
        global::NUnit.Framework.Assert.That(task.Destination, Is.EqualTo("c:\\deploy"));
    }

    [Test]
    public void HookEventCollection_Add_AddsLocationWithDefaultTimeout()
    {
        var hooks = new HookEventCollection();

        hooks.Add("scripts/start.ps1");

        var hook = hooks.Single();
        global::NUnit.Framework.Assert.That(hook.Location, Is.EqualTo("scripts/start.ps1"));
        global::NUnit.Framework.Assert.That(hook.Timeout, Is.EqualTo(3600));
    }
}
