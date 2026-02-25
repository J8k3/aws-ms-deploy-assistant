using AWSDeploymentAssistant;
using DeploymentAssert = AWSDeploymentAssistant.Assert;

namespace AWSDeploymentAssistant.Tests;

public class AssertTests
{
    [Test]
    public void IsNotNullOrEmptyString_Throws_ForEmptyValue()
    {
        var ex = global::NUnit.Framework.Assert.Throws<System.ArgumentException>(() => DeploymentAssert.IsNotNullOrEmptyString(string.Empty));

        global::NUnit.Framework.Assert.That(ex!.Message, Does.Contain("expected to not be a null or empty string"));
    }

    [Test]
    public void StartsWith_DoesNotThrow_ForMatchingPrefix_IgnoringCase()
    {
        global::NUnit.Framework.Assert.DoesNotThrow(() => DeploymentAssert.StartsWith("ReleaseCandidate", "release"));
    }

    [Test]
    public void EndsWith_Throws_ForMismatchedSuffix()
    {
        var ex = global::NUnit.Framework.Assert.Throws<System.ArgumentException>(() => DeploymentAssert.EndsWith("archive.zip", ".tar"));

        global::NUnit.Framework.Assert.That(ex!.Message, Does.Contain("does not end"));
    }

    [Test]
    public void StringDoesNotEqual_Throws_WhenValuesMatch()
    {
        var ex = global::NUnit.Framework.Assert.Throws<System.ArgumentException>(() => DeploymentAssert.StringDoesNotEqual("Deploy", "deploy"));

        global::NUnit.Framework.Assert.That(ex!.Message, Does.Contain("should not equal"));
    }

    [Test]
    public void IsWhitelistedValue_DoesNotThrow_ForAllowedValue_IgnoringCase()
    {
        var whitelist = new[] { "Debug", "Release", "Deploy" };

        global::NUnit.Framework.Assert.DoesNotThrow(() => DeploymentAssert.IsWhitelistedValue("release", whitelist));
    }

    [Test]
    public void IsWhitelistedValue_Throws_ForDisallowedValue()
    {
        var whitelist = new[] { "Debug", "Release" };

        var ex = global::NUnit.Framework.Assert.Throws<System.ArgumentException>(() => DeploymentAssert.IsWhitelistedValue("Deploy", whitelist));

        global::NUnit.Framework.Assert.That(ex!.Message, Does.Contain("not permitted"));
    }
}
