using System.Collections.Generic;
using AWSDeploymentAssistant;

namespace AWSDeploymentAssistant.Tests;

public class ExtensionsTests
{
    [Test]
    public void AddRange_AppendsAllSourceElements()
    {
        ICollection<int> target = new List<int> { 1 };
        var source = new[] { 2, 3 };

        target.AddRange(source);

        global::NUnit.Framework.Assert.That(target, Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void AddRange_Throws_WhenTargetIsNull()
    {
        ICollection<int>? target = null;

        var ex = global::NUnit.Framework.Assert.Throws<System.ArgumentNullException>(() => Extensions.AddRange(target!, new[] { 1 }));

        global::NUnit.Framework.Assert.That(ex!.ParamName, Is.EqualTo("target"));
    }
}
