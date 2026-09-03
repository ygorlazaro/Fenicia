using AwesomeAssertions;
using Fenicia.Common.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fenicia.Common.API.Tests.Middlewares;

public class WideEventMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldSetPathAndMethod()
    {
        var middleware = new WideEventMiddleware(_ => Task.CompletedTask, Mock.Of<ILogger<WideEventMiddleware>>());
        var context = new DefaultHttpContext();
        var wide = new API.WideEventContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "POST";

        await middleware.InvokeAsync(context, wide);

        wide.Path.Should().Be("/api/test");
        wide.Method.Should().Be("POST");
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetStatusCodeOnSuccess()
    {
        var middleware = new WideEventMiddleware(_ => Task.CompletedTask, Mock.Of<ILogger<WideEventMiddleware>>());
        var context = new DefaultHttpContext();
        var wide = new API.WideEventContext();
        context.Response.StatusCode = 200;

        await middleware.InvokeAsync(context, wide);

        wide.StatusCode.Should().Be(200);
        wide.Success.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetStatusCodeOnError()
    {
        var middleware = new WideEventMiddleware(
            _ => throw new InvalidOperationException("Test error"),
            Mock.Of<ILogger<WideEventMiddleware>>());
        var context = new DefaultHttpContext();
        var wide = new API.WideEventContext();

        var act = () => middleware.InvokeAsync(context, wide);

        await act.Should().ThrowAsync<InvalidOperationException>();
        wide.Success.Should().BeFalse();
        wide.StatusCode.Should().Be(500);
        wide.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetDurationMs()
    {
        var middleware = new WideEventMiddleware(_ => Task.CompletedTask, Mock.Of<ILogger<WideEventMiddleware>>());
        var context = new DefaultHttpContext();
        var wide = new API.WideEventContext();

        await middleware.InvokeAsync(context, wide);

        wide.DurationMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetOperation()
    {
        var middleware = new WideEventMiddleware(_ => Task.CompletedTask, Mock.Of<ILogger<WideEventMiddleware>>());
        var context = new DefaultHttpContext();
        var wide = new API.WideEventContext();
        context.Request.Path = "/api/users";
        context.Request.Method = "GET";

        await middleware.InvokeAsync(context, wide);

        wide.Operation.Should().Be("/api/users GET");
    }
}