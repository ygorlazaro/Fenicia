using Fenicia.Common.Exceptions;

namespace Fenicia.Common.Tests;

[TestFixture]
public class ExceptionsTests
{
    [Test]
    public void NotSavedException_Should_Inherit_Exception_And_Contain_Message()
    {
        var ex = new NotSavedException("not saved");
        Assert.That(ex, Is.InstanceOf<Exception>());
        Assert.That(ex.Message, Is.EqualTo("not saved"));
    }

    [Test]
    public void ItemNotExistsException_Should_Inherit_Exception_And_Contain_Message()
    {
        var ex = new ItemNotExistsException("missing");
        Assert.That(ex, Is.InstanceOf<Exception>());
        Assert.That(ex.Message, Is.EqualTo("missing"));
    }

    [Test]
    public void PermissionDeniedException_Should_Inherit_Exception_And_Contain_Message()
    {
        var ex = new PermissionDeniedException("denied");
        Assert.That(ex, Is.InstanceOf<Exception>());
        Assert.That(ex.Message, Is.EqualTo("denied"));
    }
}
