using System.Security.Claims;
using AwesomeAssertions;
using Fenicia.Common.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fenicia.Common.API.Tests.Middlewares;

public class ModuleRequirementMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenModuleClaimMissing()
    {
        var requestDelegate = new Mock<RequestDelegate>();
        var middleware = new ModuleRequirementMiddleware(requestDelegate.Object, "Auth");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([]))
        };

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenModuleNotInClaim()
    {
        var requestDelegate = new Mock<RequestDelegate>();
        var middleware = new ModuleRequirementMiddleware(requestDelegate.Object, "Auth");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("module", "[\"Basic\"]")
                ]))
        };

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenModuleIsInClaim()
    {
        var called = false;
        var middleware = new ModuleRequirementMiddleware(
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            "Auth");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("module", "[\"Auth\",\"Basic\"]")
                ]))
        };

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn403_WhenClaimFormatIsInvalid()
    {
        var requestDelegate = new Mock<RequestDelegate>();
        var middleware = new ModuleRequirementMiddleware(requestDelegate.Object, "Auth");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim("module", "invalid-json")
                ]))
        };

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}