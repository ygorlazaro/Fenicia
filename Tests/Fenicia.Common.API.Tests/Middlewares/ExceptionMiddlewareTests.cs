using System.Net;
using AwesomeAssertions;
using Fenicia.Common.API.Middlewares;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn500_ForGenericException()
    {
        var middleware = CreateMiddleware();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnJsonResponse_ForGenericException()
    {
        var middleware = CreateMiddleware();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.ContentType.Should().Contain("application/json");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn400_ForInvalidRequestException()
    {
        var middleware = CreateMiddleware(() => new InvalidRequestException("Invalid request"));
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn404_ForItemNotExistsException()
    {
        var middleware = CreateMiddleware(() => new ItemNotExistsException("Item not found"));
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_ForPermissionDeniedException()
    {
        var middleware = CreateMiddleware(() => new PermissionDeniedException("Permission denied"));
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn401_ForUnauthorizedAccessException()
    {
        var middleware = CreateMiddleware(() => new UnauthorizedAccessException("Unauthorized"));
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnErrorCode_ForInvalidRequestException()
    {
        var middleware = CreateMiddleware(() => new InvalidRequestException("Invalid request"));
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.InvokeAsync(context);

        var responseBody = await ReadResponseBodyAsync(context.Response);
        responseBody.Should().Contain("InvalidRequest");
    }

    private static ExceptionMiddleware CreateMiddleware(Func<Exception>? exceptionFactory = null)
    {
        var companyContext = new TestCompanyContext();
        var next = new RequestDelegate(_ =>
        {
            if (exceptionFactory is not null)
            {
                throw exceptionFactory();
            }

            throw new InvalidOperationException("Test exception");
        });

        return new ExceptionMiddleware(next, companyContext);
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body);
        return await reader.ReadToEndAsync();
    }
}
