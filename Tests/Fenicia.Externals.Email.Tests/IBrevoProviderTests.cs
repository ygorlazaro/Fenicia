using AwesomeAssertions;

namespace Fenicia.Externals.Email.Tests;

public class IBrevoProviderTests
{
    [Fact]
    public void Interface_ShouldHaveSendMethod()
    {
        var interfaceType = typeof(IBrevoProvider);

        var method = interfaceType.GetMethod("Send");
        method.Should().NotBeNull();
        method.ReturnType.Should().Be(typeof(void));
    }

    [Fact]
    public void SendMethod_ShouldHaveCorrectParameters()
    {
        var interfaceType = typeof(IBrevoProvider);
        var method = interfaceType.GetMethod("Send")!;

        method.GetParameters().Should().HaveCount(4);
        method.GetParameters()[0].Name.Should().Be("template");
        method.GetParameters()[1].Name.Should().Be("email");
        method.GetParameters()[2].Name.Should().Be("name");
        method.GetParameters()[3].Name.Should().Be("parameters");
    }
}
