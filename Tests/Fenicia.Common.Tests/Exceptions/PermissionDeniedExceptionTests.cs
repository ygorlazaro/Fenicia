using AwesomeAssertions;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Common.Tests.Exceptions;

public class PermissionDeniedExceptionTests
{
    [Fact]
    public void Constructor_WithNoMessage_ShouldUseDefaultMessage()
    {
        var exception = new PermissionDeniedException();

        exception.Message.Should().Be(ExceptionMessages.PermissionDenied);
    }

    [Fact]
    public void Constructor_WithCustomMessage_ShouldUseCustomMessage()
    {
        const string customMessage = "Custom permission denied message";
        var exception = new PermissionDeniedException(customMessage);

        exception.Message.Should().Be(customMessage);
    }
}