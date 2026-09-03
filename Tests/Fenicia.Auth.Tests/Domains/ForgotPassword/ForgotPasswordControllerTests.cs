using System.Net;
using System.Security.Claims;
using Bogus;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ForgotPasswordControllerTests
{
    private readonly ForgotPasswordController _controller;
    private readonly Faker _faker;
    private readonly Mock<IForgotPasswordService> _mockService;

    public ForgotPasswordControllerTests()
    {
        var testUserId = Guid.NewGuid();
        _faker = new Faker();
        _mockService = new Mock<IForgotPasswordService>();

        var httpContext = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = IPAddress.Parse("127.0.0.1")
            }
        };
        httpContext.Request.Headers.UserAgent = "TestAgent";

        _controller = new ForgotPasswordController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = httpContext } };

        SetupUserClaims(testUserId);
    }

    [Fact]
    public async Task PostAsync_WhenUserExists_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddForgotPasswordCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new AddForgotPasswordCommand(email);

        var result = await _controller.PostAsync(command, wide, cancellationToken);

        Assert.IsType<CreatedResult>(result);
        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public async Task PostAsync_WhenUserDoesNotExist_ReturnsBadRequest()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddForgotPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ItemNotExistsException("User with given email does not exist."));

        var command = new AddForgotPasswordCommand(_faker.Internet.Email());

        var result = await _controller.PostAsync(command, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddForgotPasswordCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new AddForgotPasswordCommand(email);

        await _controller.PostAsync(command, wide, cancellationToken);

        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenValidCode_ResetsPasswordSuccessfully()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();
        var newPassword = _faker.Internet.Password();

        _mockService.Setup(s => s.ResetAsync(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var code = _faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        var command = new ResetPasswordCommand(email, newPassword, code);

        var result = await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenInvalidCode_ReturnsBadRequest()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();

        _mockService.Setup(s => s.ResetAsync(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("Invalid forgot password code."));

        var command = new ResetPasswordCommand(email, _faker.Internet.Password(), "INVALID");

        var result = await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PatchAsync_WhenUserDoesNotExist_ReturnsBadRequest()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.ResetAsync(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ItemNotExistsException("User with given email does not exist."));

        var command = new ResetPasswordCommand(
            _faker.Internet.Email(),
            _faker.Internet.Password(),
            _faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));

        var result = await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var email = _faker.Internet.Email();
        var newPassword = _faker.Internet.Password();

        _mockService.Setup(s => s.ResetAsync(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var code = _faker.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        var command = new ResetPasswordCommand(email, newPassword, code);

        await _controller.PatchAsync(command, wide, cancellationToken);

        Assert.Equal(command.Email, wide.UserId);
    }

    [Fact]
    public void ForgotPasswordController_HasAllowAnonymousAttribute()
    {
        var controllerType = typeof(ForgotPasswordController);

        var allowAnonymousAttribute =
            controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        Assert.NotNull(allowAnonymousAttribute);
    }

    [Fact]
    public void ForgotPasswordController_HasRouteAttribute()
    {
        var controllerType = typeof(ForgotPasswordController);

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ForgotPasswordController_HasProducesAttribute()
    {
        var controllerType = typeof(ForgotPasswordController);

        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}