using AwesomeAssertions;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Common.Tests.Exceptions;

public class InvalidRequestExceptionTests
{
    [Fact]
    public void Constructor_WithNoMessage_ShouldUseDefaultMessage()
    {
        var exception = new InvalidRequestException();

        exception.Message.Should().Be(ExceptionMessages.InvalidRequest);
    }

    [Fact]
    public void Constructor_WithCustomMessage_ShouldUseCustomMessage()
    {
        var customMessage = "Custom invalid request message";
        var exception = new InvalidRequestException(customMessage);

        exception.Message.Should().Be(customMessage);
    }
}
