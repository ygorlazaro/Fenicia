using AwesomeAssertions;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Common.Tests.Exceptions;

public class ItemNotExistsExceptionTests
{
    [Fact]
    public void Constructor_WithNoMessage_ShouldUseDefaultMessage()
    {
        var exception = new ItemNotExistsException();

        exception.Message.Should().Be(ExceptionMessages.ItemNotExists);
    }

    [Fact]
    public void Constructor_WithCustomMessage_ShouldUseCustomMessage()
    {
        var customMessage = "Custom item not found message";
        var exception = new ItemNotExistsException(customMessage);

        exception.Message.Should().Be(customMessage);
    }
}
