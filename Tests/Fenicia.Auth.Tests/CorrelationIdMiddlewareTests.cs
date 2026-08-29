using Bogus;

using Fenicia.Common.API;

using Microsoft.AspNetCore.Http;

namespace Fenicia.Auth.Tests;

public class CorrelationIdMiddlewareTests
{
    private const string _headerName = "X-Correlation-ID";

    private readonly Faker _faker = new();

    [Fact]
    public async Task InvokeAsync_WhenHeaderPresent_PassesThroughAndSetsResponseHeader()
    {
        var context = new DefaultHttpContext();
        var existing = _faker.Random.Guid().ToString();
        context.Request.Headers[_headerName] = existing;

        var called = false;

        var middleware = new CorrelationIdMiddleware(Next);

        await middleware.InvokeAsync(context);

        Assert.True(called, "Next delegate should be invoked");
        Assert.True(context.Response.Headers.ContainsKey(_headerName), "Response should contain correlation header");
        Assert.Equal(existing, context.Response.Headers[_headerName].ToString());

        return;

        Task Next(HttpContext ctx)
        {
            called = true;
            Assert.Equal(existing, ctx.Request.Headers[_headerName].ToString());
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenHeaderMissing_GeneratesCorrelationIdAndSetsRequestAndResponse()
    {
        var context = new DefaultHttpContext();

        var called = false;

        var middleware = new CorrelationIdMiddleware(Next);

        await middleware.InvokeAsync(context);

        Assert.True(called, "Next delegate should be invoked");
        Assert.True(context.Request.Headers.ContainsKey(_headerName), "Request should contain generated correlation header");
        Assert.True(context.Response.Headers.ContainsKey(_headerName), "Response should contain correlation header");

        var value = context.Request.Headers[_headerName].ToString();
        Assert.False(string.IsNullOrWhiteSpace(value), "Generated correlation id should not be empty");
        Assert.True(Guid.TryParse(value, out _), "Generated correlation id should be a valid GUID");

        return;

        Task Next(HttpContext ctx)
        {
            called = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenNextThrows_ExceptionIsRethrownAndHeaderIsSet()
    {
        var context = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(Next);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await middleware.InvokeAsync(context));
        Assert.Equal("boom", ex.Message);
        Assert.True(context.Response.Headers.ContainsKey(_headerName), "Response should contain correlation header even when an exception is thrown");

        return;

        static Task Next(HttpContext ctx) => throw new InvalidOperationException("boom");
    }
}
