using System.Security.Claims;
using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IModuleService> _mockModuleService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Guid _testUserId;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockModuleService = new Mock<IModuleService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();

        _controller = new UserController(_mockUserService.Object, _mockModuleService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasNoModules_ReturnsOkWithEmptyList()
    {
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockModuleService.Setup(s => s.GetUserModulesAsync(companyId, _testUserId, cancellationToken))
            .ReturnsAsync([]);

        var result = await _controller.GetUserModulesAsync(_testUserId, headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedModules = Assert.IsType<List<GetUserModulesResponse>>(okResult.Value);
        Assert.Empty(returnedModules);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(
                _testUserId,
                otherUserId,
                headers.CompanyId,
                cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_ButIsAdmin_ReturnsOk2()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(
                _testUserId,
                otherUserId,
                headers.CompanyId,
                cancellationToken))
            .Returns(Task.CompletedTask);
        _mockModuleService.Setup(s => s.GetUserModulesAsync(companyId, otherUserId, cancellationToken))
            .ReturnsAsync([]);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, cancellationToken);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_AdminInDifferentCompany_ReturnsForbid()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(
                _testUserId,
                otherUserId,
                headers.CompanyId,
                cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserHasNoCompanies_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.GetCompaniesAsync(_testUserId, cancellationToken))
            .ReturnsAsync([]);

        var result = await _controller.GetUserCompanyAsync(_testUserId, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedCompanies = Assert.IsType<List<GetUserCompaniesResponse>>(okResult.Value);
        Assert.Empty(returnedCompanies);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserHasCompanies_ReturnsOkWithCompanies()
    {
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.GetCompaniesAsync(_testUserId, cancellationToken))
            .ReturnsAsync([new GetUserCompaniesResponse(companyId, role.Name, companyId, company.Name, company.Cnpj)]);

        var result = await _controller.GetUserCompanyAsync(_testUserId, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedCompanies = Assert.IsType<List<GetUserCompaniesResponse>>(okResult.Value);
        Assert.Single(returnedCompanies);
        Assert.Equal(companyId, returnedCompanies[0].Id);
        Assert.Equal("Admin", returnedCompanies[0].Role);
        Assert.Equal(company.Name, returnedCompanies[0].CompanyName);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.GetCompaniesAsync(_testUserId, cancellationToken))
            .ReturnsAsync([]);

        await _controller.GetUserCompanyAsync(_testUserId, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid2()
    {
        var otherUserId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(_testUserId, otherUserId, null, cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_ButSharesCompany_ReturnsOk()
    {
        var otherUserId = Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(_testUserId, otherUserId, null, cancellationToken))
            .Returns(Task.CompletedTask);
        _mockUserService.Setup(s => s.GetCompaniesAsync(otherUserId, cancellationToken))
            .ReturnsAsync([]);

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, cancellationToken);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid2()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(
                _testUserId,
                otherUserId,
                headers.CompanyId,
                cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_ButIsAdmin_ReturnsOk()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(
                _testUserId,
                otherUserId,
                headers.CompanyId,
                cancellationToken))
            .Returns(Task.CompletedTask);
        _mockModuleService.Setup(s => s.GetUserModulesAsync(companyId, otherUserId, cancellationToken))
            .ReturnsAsync([]);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, cancellationToken);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid()
    {
        var otherUserId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(_testUserId, otherUserId, null, cancellationToken))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_ButIsAdmin_ReturnsOk()
    {
        var otherUserId = Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockUserService.Setup(s => s.EnsureCanAccessUserAsync(_testUserId, otherUserId, null, cancellationToken))
            .Returns(Task.CompletedTask);
        _mockUserService.Setup(s => s.GetCompaniesAsync(otherUserId, cancellationToken))
            .ReturnsAsync([]);

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, cancellationToken);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void UserController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(UserController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void UserController_HasRouteAttribute()
    {
        var controllerType = typeof(UserController);

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void UserController_HasApiControllerAttribute()
    {
        var controllerType = typeof(UserController);

        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public async Task GetAsync_WithGodRole_ReturnsOkWithUsers()
    {
        SetupUserClaims(_testUserId, "God");

        _mockUserService.Setup(s => s.GetAllAsync(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<UserListItemResponse>>([], 1, 1, 10));

        var result = await _controller.GetAsync(1, 10, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithGodRole_ReturnsFullUserData()
    {
        SetupUserClaims(_testUserId, "God");

        Guid.NewGuid();
        Guid.NewGuid();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _mockUserService.Setup(s => s.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserByIdResponse(user.Id, user.Name, user.Email));

        var result = await _controller.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        _mockUserService.Setup(s => s.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetUserByIdResponse?)null);

        var result = await _controller.GetByIdAsync(nonExistentUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        _mockUserService.Setup(s => s.DeleteAsync(nonExistentUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException("User not found"));

        var result = await _controller.DeleteAsync(nonExistentUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttemptingSelfDeletion_ReturnsBadRequest()
    {
        SetupUserClaims(_testUserId, "God");

        _mockUserService.Setup(s => s.DeleteAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException("Cannot delete yourself"));

        var result = await _controller.DeleteAsync(_testUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserPasswordCommand(_testUserId, _faker.Internet.Password());

        _mockUserService.Setup(s => s.UpdatePasswordAsync(
                It.IsAny<UpdateUserPasswordCommand>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidRequestException("User not found"));

        var result = await _controller.ChangePasswordAsync(nonExistentUserId, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    private void SetupUserClaims(Guid userId, string? role = null)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}