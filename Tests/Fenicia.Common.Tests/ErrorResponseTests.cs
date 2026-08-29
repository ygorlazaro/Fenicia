using FluentAssertions;

namespace Fenicia.Common.Tests;

public class ErrorResponseTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var errorResponse = new ErrorResponse
        {
            Message = "Test error message"
        };

        errorResponse.Message.Should().Be("Test error message");
    }

    [Fact]
    public void Message_ShouldBeNullable()
    {
        var errorResponse = new ErrorResponse();

        errorResponse.Message.Should().BeNull();
    }
}
