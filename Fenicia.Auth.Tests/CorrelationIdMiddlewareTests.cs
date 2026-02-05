using Microsoft.AspNetCore.Http;

namespace Fenicia.Auth.Tests;

[TestFixture]
public class CorrelationIdMiddlewareTests
{
    private const string HeaderName = "X-Correlation-ID";

    [Test]
    public async Task InvokeAsync_WhenHeaderPresent_PassesThroughAndSetsResponseHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var existing = "existing-correlation-id";
        context.Request.Headers[HeaderName] = existing;

        var called = false;
        RequestDelegate next = ctx =>
        {
            called = true;
            // ensure request header still present
            Assert.That(ctx.Request.Headers[HeaderName].ToString(), Is.EqualTo(existing));
            return Task.CompletedTask;
        };

        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(called, Is.True, "Next delegate should be invoked");
        Assert.That(context.Response.Headers.ContainsKey(HeaderName), Is.True, "Response should contain correlation header");
        Assert.That(context.Response.Headers[HeaderName].ToString(), Is.EqualTo(existing));
    }

    [Test]
    public async Task InvokeAsync_WhenHeaderMissing_GeneratesCorrelationIdAndSetsRequestAndResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var called = false;
        RequestDelegate next = ctx =>
        {
            called = true;
            return Task.CompletedTask;
        };

        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(called, Is.True, "Next delegate should be invoked");
        Assert.That(context.Request.Headers.ContainsKey(HeaderName), Is.True, "Request should contain generated correlation header");
        Assert.That(context.Response.Headers.ContainsKey(HeaderName), Is.True, "Response should contain correlation header");

        var value = context.Request.Headers[HeaderName].ToString();
        Assert.That(string.IsNullOrWhiteSpace(value), Is.False, "Generated correlation id should not be empty");
        Assert.That(value, Is.EqualTo(context.Response.Headers[HeaderName].ToString()), "Request and Response correlation ids should match");
        Assert.That(Guid.TryParse(value, out _), Is.True, "Generated correlation id should be a valid GUID");
    }

    [Test]
    public void InvokeAsync_WhenNextThrows_ExceptionIsRethrownAndHeaderIsSet()
    {
        // Arrange
        var context = new DefaultHttpContext();

        RequestDelegate next = ctx => throw new InvalidOperationException("boom");

        var middleware = new CorrelationIdMiddleware(next);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await middleware.InvokeAsync(context));
        Assert.That(ex?.Message, Is.EqualTo("boom"));

        // Even when next throws, middleware should have set the response header
        Assert.That(context.Response.Headers.ContainsKey(HeaderName), Is.True, "Response should contain correlation header even when an exception is thrown");
    }
}
