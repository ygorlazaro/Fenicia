using Microsoft.AspNetCore.Http;

namespace Fenicia.Auth.Tests;

[TestFixture]
public class CorrelationIdMiddlewareTests
{
    private const string HeaderName = "X-Correlation-ID";

    [Test]
    public async Task InvokeAsync_WhenHeaderPresent_PassesThroughAndSetsResponseHeader()
    {
        var context = new DefaultHttpContext();
        const string existing = "existing-correlation-id";
        context.Request.Headers[HeaderName] = existing;

        var called = false;

        var middleware = new CorrelationIdMiddleware(Next);

        await middleware.InvokeAsync(context);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.True, "Next delegate should be invoked");
            Assert.That(context.Response.Headers.ContainsKey(HeaderName), Is.True, "Response should contain correlation header");
            Assert.That(context.Response.Headers[HeaderName].ToString(), Is.EqualTo(existing));
        }

        return;

        Task Next(HttpContext ctx)
        {
            called = true;
            Assert.That(ctx.Request.Headers[HeaderName].ToString(), Is.EqualTo(existing));
            return Task.CompletedTask;
        }
    }

    [Test]
    public async Task InvokeAsync_WhenHeaderMissing_GeneratesCorrelationIdAndSetsRequestAndResponse()
    {
        var context = new DefaultHttpContext();

        var called = false;

        var middleware = new CorrelationIdMiddleware(Next);

        await middleware.InvokeAsync(context);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.True, "Next delegate should be invoked");
            Assert.That(context.Request.Headers.ContainsKey(HeaderName), Is.True, "Request should contain generated correlation header");
            Assert.That(context.Response.Headers.ContainsKey(HeaderName), Is.True, "Response should contain correlation header");
        }

        var value = context.Request.Headers[HeaderName].ToString();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(string.IsNullOrWhiteSpace(value), Is.False, "Generated correlation id should not be empty");
            Assert.That(value, Is.EqualTo(context.Response.Headers[HeaderName].ToString()), "Request and Response correlation ids should match");
            Assert.That(Guid.TryParse(value, out _), Is.True, "Generated correlation id should be a valid GUID");
        }

        return;

        Task Next(HttpContext ctx)
        {
            called = true;
            return Task.CompletedTask;
        }
    }

    [Test]
    public void InvokeAsync_WhenNextThrows_ExceptionIsRethrownAndHeaderIsSet()
    {
        var context = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(Next);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await middleware.InvokeAsync(context));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex?.Message, Is.EqualTo("boom"));

            Assert.That(context.Response.Headers.ContainsKey(HeaderName), Is.True, "Response should contain correlation header even when an exception is thrown");
        }

        return;

        Task Next(HttpContext ctx) => throw new InvalidOperationException("boom");
    }
}
