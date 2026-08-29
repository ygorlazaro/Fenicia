using System.Security.Claims;

using Fenicia.Common.API.Middlewares;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Tests.Middlewares;

public class RoleGodRequirementMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenRoleClaimMissing()
    {
        var middleware = new RoleGodRequirementMiddleware(next: null!);
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([]));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenRoleIsNotAdmin()
    {
        var middleware = new RoleGodRequirementMiddleware(next: null!);
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("role", "[\"User\"]")
        }));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenRoleIsAdmin()
    {
        var called = false;
        var middleware = new RoleGodRequirementMiddleware(_ =>
        {
            called = true;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("role", "[\"Admin\"]")
        }));

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenClaimFormatIsInvalid()
    {
        var middleware = new RoleGodRequirementMiddleware(next: null!);
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("role", "invalid-json")
        }));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}
