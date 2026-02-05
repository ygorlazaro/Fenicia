using Fenicia.Common.API;

namespace Fenicia.Common.Tests.Api;

[TestFixture]
public class AppSettingsReaderTests
{
    [Test]
    public void GetConnectionString_Returns_Expected_Value()
    {
        var value = AppSettingsReader.GetConnectionString("Auth");
        Assert.That(value, Does.Contain("fenicia-auth-god"));
    }

    [Test]
    public void GetConnectionString_Invalid_Throws_InvalidOperationException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => AppSettingsReader.GetConnectionString("NoSuchKey"));
        Assert.That(ex.Message, Does.Contain("Connection string 'NoSuchKey' not found"));
        Assert.That(ex.Message, Does.Contain("Available keys:"));
    }
}
