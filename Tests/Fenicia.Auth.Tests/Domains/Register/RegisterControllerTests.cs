using System.Security.Claims;
using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Register;

public class RegisterControllerTests
{
    private readonly RegisterController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IRegisterService> _mockService;

    public RegisterControllerTests()
    {
        var adminRoleId = Guid.NewGuid();
        _mockHttpContext = new Mock<HttpContext>();
        _mockService = new Mock<IRegisterService>();

        _controller = new RegisterController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(adminRoleId);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var wide = new Common.API.WideEventContext();
        var cancellationToken = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("existing@example.com", "password123", "Test User", company);

        _mockService.Setup(s => s.CreateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException("This email already exists"));

        var result = await _controller.CreateNewUserAsync(request, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        var wide = new Common.API.WideEventContext();
        var cancellationToken = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Existing Company");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        _mockService.Setup(s => s.CreateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException("Company with this CNPJ already exists."));

        var result = await _controller.CreateNewUserAsync(request, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenAdminRoleDoesNotExist_ReturnsBadRequest()
    {
        var wide = new Common.API.WideEventContext();
        var cancellationToken = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        _mockService.Setup(s => s.CreateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException("Admin role not found. Please ensure that the admin role exists in the database."));

        var result = await _controller.CreateNewUserAsync(request, wide, cancellationToken);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewUserAsync_WhenValidRequest_ReturnsCreatedWithUser()
    {
        var wide = new Common.API.WideEventContext();
        var cancellationToken = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        var expectedResponse = new RegisterResponse(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            new CreateNewUserCompanyResponse(Guid.NewGuid(), "Company Name", "12.345.678/0001-90"));

        _mockService.Setup(s => s.CreateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.CreateNewUserAsync(request, wide, cancellationToken);

        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var response = Assert.IsType<RegisterResponse>(createdResult.Value);

        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(company.Name, response.Company.Name);
        Assert.Equal(request.Email, wide.UserId);
    }

    [Fact]
    public async Task CreateNewUserAsync_SetsWideEventContextUserId()
    {
        var wide = new Common.API.WideEventContext();
        var cancellationToken = CancellationToken.None;
        var company = new CreateNewUserCompanyCommand("12.345.678/0001-90", "Company Name");
        var request = new RegisterCommand("test@example.com", "password123", "Test User", company);

        _mockService.Setup(s => s.CreateAsync(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegisterResponse(Guid.NewGuid(), "Test User", "test@example.com", new CreateNewUserCompanyResponse(Guid.NewGuid(), "Company Name", "12.345.678/0001-90")));

        await _controller.CreateNewUserAsync(request, wide, cancellationToken);

        Assert.Equal(request.Email, wide.UserId);
    }

    [Fact]
    public void RegisterController_HasAllowAnonymousAttribute()
    {
        Assert.NotNull(typeof(RegisterController).GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault());
    }

    [Fact]
    public void RegisterController_HasRouteAttribute()
    {
        var route = typeof(RegisterController).GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
        Assert.NotNull(route);
        Assert.Equal("[controller]", route.Template);
    }

    [Fact]
    public void RegisterController_HasApiControllerAttribute()
    {
        Assert.NotNull(typeof(RegisterController).GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault());
    }

    [Fact]
    public void RegisterController_HasProducesAttribute()
    {
        var produces = typeof(RegisterController).GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;
        Assert.NotNull(produces);
        Assert.Equal("application/json", produces.ContentTypes.FirstOrDefault());
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
