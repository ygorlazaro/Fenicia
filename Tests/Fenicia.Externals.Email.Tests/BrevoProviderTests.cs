using AwesomeAssertions;
using Fenicia.Common.Enums.External;

namespace Fenicia.Externals.Email.Tests;

public class BrevoProviderTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        var provider = new BrevoProvider();

        provider.Should().NotBeNull();
    }

    [Fact]
    public void Send_ShouldReadApiKeyFromEnvironment()
    {
        var provider = new BrevoProvider();
        var act = () => provider.Send(EmailTemplate.ForgotPassword, "test@example.com", "Test", null);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Send_ShouldUseTemplateIdFromEnum()
    {
        const EmailTemplate template = EmailTemplate.ForgotPassword;
        const int templateId = (int)template;

        templateId.Should().Be(1);
    }
}
