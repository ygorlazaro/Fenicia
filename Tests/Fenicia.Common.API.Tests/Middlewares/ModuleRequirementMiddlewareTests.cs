using System.Security.Claims;

using Fenicia.Common.API.Middlewares;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Tests.Middlewares;

public class ModuleRequirementMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenModuleClaimMissing()
    {
        var middleware = new ModuleRequirementMiddleware(next: null!, "Auth");
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([]));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenModuleNotInClaim()
    {
        var middleware = new ModuleRequirementMiddleware(next: null!, "Auth");
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("module", "[\"Basic\"]")
        }));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenModuleIsInClaim()
    {
        var called = false;
        var middleware = new ModuleRequirementMiddleware(
            next: _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            requiredModule: "Auth");
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("module", "[\"Auth\",\"Basic\"]")
        }));

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenClaimFormatIsInvalid()
    {
        var middleware = new ModuleRequirementMiddleware(next: null!, "Auth");
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("module", "invalid-json")
        }));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}
