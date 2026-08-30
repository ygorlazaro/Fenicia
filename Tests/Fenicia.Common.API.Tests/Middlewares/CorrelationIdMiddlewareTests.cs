using AwesomeAssertions;
using Fenicia.Common.API.Middlewares;
using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Tests.Middlewares;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldGenerateCorrelationId_WhenHeaderMissing()
    {
        var middleware = new CorrelationIdMiddleware(next: _ => Task.CompletedTask);
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.Headers.Should().ContainKey("X-Correlation-ID");
        context.Response.Headers["X-Correlation-ID"].Should().NotBeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_ShouldUseExistingCorrelationId_WhenHeaderExists()
    {
        var middleware = new CorrelationIdMiddleware(next: _ => Task.CompletedTask);
        var context = new DefaultHttpContext();
        var existingId = "existing-correlation-id";
        context.Request.Headers["X-Correlation-ID"] = existingId;

        await middleware.InvokeAsync(context);

        context.Response.Headers["X-Correlation-ID"].ToString().Should().Be(existingId);
    }

    [Fact]
    public async Task InvokeAsync_ShouldSetResponseHeader()
    {
        var middleware = new CorrelationIdMiddleware(next: _ => Task.CompletedTask);
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.Headers.Should().ContainKey("X-Correlation-ID");
    }
}
