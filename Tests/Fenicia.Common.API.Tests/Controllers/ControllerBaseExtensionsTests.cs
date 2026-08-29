using Fenicia.Common.Api.Controllers;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.API.Tests.Controllers;

public class ControllerBaseExtensionsTests
{
    [Fact]
    public void ForbidWithMessage_ShouldReturnObjectResult_WithStatusCode403()
    {
        var controller = new TestController();
        var result = controller.ForbidWithMessage("Test message");

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(403);
    }

    [Fact]
    public void ForbidWithMessage_ShouldReturnProblem_WithCorrectProperties()
    {
        var controller = new TestController();
        var result = controller.ForbidWithMessage("Test message") as ObjectResult;

        result.Should().NotBeNull();
        var valueType = result!.Value!.GetType()!;
        valueType.GetProperty("status")!.GetValue(result.Value!)!.Should().Be(403);
        valueType.GetProperty("title")!.GetValue(result.Value!)!.Should().Be("Forbidden");
        valueType.GetProperty("detail")!.GetValue(result.Value!)!.Should().Be("Test message");
    }

    private sealed class TestController : ControllerBase
    {
    }
}
