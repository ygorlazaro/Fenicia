using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using AwesomeAssertions;

namespace Fenicia.Common.Tests.Exceptions;

public class NotSavedExceptionTests
{
    [Fact]
    public void Constructor_WithNoMessage_ShouldUseDefaultMessage()
    {
        var exception = new NotSavedException();

        exception.Message.Should().Be(ExceptionMessages.NotSaved);
    }

    [Fact]
    public void Constructor_WithCustomMessage_ShouldUseCustomMessage()
    {
        const string customMessage = "Custom not saved message";
        var exception = new NotSavedException(customMessage);

        exception.Message.Should().Be(customMessage);
    }
}
