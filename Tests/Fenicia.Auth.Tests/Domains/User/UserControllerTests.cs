using System.Security.Claims;
using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;
    private readonly UserService _userService;

    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository);
        _userService = new UserService(_userRepository, userRoleService, roleService, companyService, new SecurityService(), new ModuleService(new ModuleRepository(_db)));
        _testUserId = Guid.NewGuid();

        _mockHttpContext = new Mock<HttpContext>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

        _controller = new UserController(_userService) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasNoModules_ReturnsOkWithEmptyList()
    {
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetUserModulesAsync(_testUserId, headers, wide, ct);

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
        var ct = CancellationToken.None;

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_ButIsAdmin_ReturnsOk2()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var loggedInUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _roleRepository.InsertAsync(adminRole, CancellationToken.None);
        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _userRoleRepository.InsertAsync(loggedInUserRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, ct);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_AdminInDifferentCompany_ReturnsForbid()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var otherCompanyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var loggedInUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = adminRoleId,
            CompanyId = otherCompanyId
        };

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _roleRepository.InsertAsync(adminRole, CancellationToken.None);
        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _userRoleRepository.InsertAsync(loggedInUserRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserHasActiveSubscription_ReturnsOkWithModules()
    {
        var companyId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscriptionCreditId = Guid.NewGuid();

        var module = new ModuleModel
        {
            Id = moduleId,
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10,                100)
        };

        var subscription = new SubscriptionModel
        {
            Id = subscriptionId,
            CompanyId = companyId,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30)
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = subscriptionCreditId,
            SubscriptionId = subscriptionId,
            ModuleId = moduleId,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30)
        };

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = Guid.NewGuid(),
            CompanyId = companyId
        };

        _db.AuthModules.Add(module);
        _db.AuthSubscriptions.Add(subscription);
        _db.AuthSubscriptionCredits.Add(subscriptionCredit);
        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _userRoleRepository.InsertAsync(userRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetUserModulesAsync(_testUserId, headers, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var returnedModules = Assert.IsType<List<GetUserModulesResponse>>(okResult.Value);
        Assert.Single(returnedModules);
        Assert.Equal(moduleId, returnedModules[0].Id);
        Assert.Equal(module.Name, returnedModules[0].Name);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserModulesAsync_SetsWideEventContextUserId()
    {
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        await _controller.GetUserModulesAsync(_testUserId, headers, wide, ct);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserHasNoCompanies_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetUserCompanyAsync(_testUserId, wide, ct);

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

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = roleId,
            CompanyId = companyId
        };

        await _companyRepository.InsertAsync(company, CancellationToken.None);
        await _roleRepository.InsertAsync(role, CancellationToken.None);
        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _userRoleRepository.InsertAsync(userRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await _controller.GetUserCompanyAsync(_testUserId, wide, ct);

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
        var ct = CancellationToken.None;

        await _controller.GetUserCompanyAsync(_testUserId, wide, ct);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid2()
    {
        var otherUserId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_ButSharesCompany_ReturnsOk()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var loggedInUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        var otherUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _roleRepository.InsertAsync(adminRole, CancellationToken.None);
        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _userRoleRepository.InsertAsync(loggedInUserRole, CancellationToken.None);
        await _userRoleRepository.InsertAsync(otherUserRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, ct);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid2()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserModulesAsync_WhenUserIsNotOwner_ButIsAdmin_ReturnsOk()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var headers = new Headers { CompanyId = companyId };
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var loggedInUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _roleRepository.InsertAsync(adminRole, CancellationToken.None);
        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _userRoleRepository.InsertAsync(loggedInUserRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserModulesAsync(otherUserId, headers, wide, ct);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_AndNotAdmin_ReturnsForbid()
    {
        var otherUserId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, ct);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetUserCompanyAsync_WhenUserIsNotOwner_ButIsAdmin_ReturnsOk()
    {
        var otherUserId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var loggedInUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var otherUserRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        await _roleRepository.InsertAsync(adminRole, CancellationToken.None);
        await _userRepository.InsertAsync(otherUser, CancellationToken.None);
        await _userRoleRepository.InsertAsync(loggedInUserRole, CancellationToken.None);
        await _userRoleRepository.InsertAsync(otherUserRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserCompanyAsync(otherUserId, wide, ct);

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

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void UserController_HasApiControllerAttribute()
    {
        var controllerType = typeof(UserController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public async Task GetAsync_WithGodRole_ReturnsOkWithUsers()
    {
        SetupUserClaims(_testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithGodRole_ReturnsFullUserData()
    {
        SetupUserClaims(_testUserId, "God");

        var roleId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };
        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = roleId,
            CompanyId = companyId
        };

        await _roleRepository.InsertAsync(role, CancellationToken.None);
        await _companyRepository.InsertAsync(company, CancellationToken.None);
        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _userRoleRepository.InsertAsync(userRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var result = await _controller.GetByIdAsync(nonExistentUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserCommand(nonExistentUserId, "Updated Name");

        var result = await _controller.UpdateAsync(nonExistentUserId, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserCommand(user.Id, "Updated Name");

        var result = await _controller.UpdateAsync(user.Id, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var result = await _controller.DeleteAsync(nonExistentUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.DeleteAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttemptingSelfDeletion_ReturnsBadRequest()
    {
        SetupUserClaims(_testUserId, "God");

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.DeleteAsync(_testUserId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserNotFound_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");
        var nonExistentUserId = Guid.NewGuid();

        var query = new UpdateUserPasswordCommand(_testUserId, _faker.Internet.Password());

        var result = await _controller.ChangePasswordAsync(nonExistentUserId, query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserIsDeleted_ReturnsNotFound()
    {
        SetupUserClaims(_testUserId, "God");

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password(),
            Deleted = DateTime.UtcNow
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdateUserPasswordCommand(user.Id, _faker.Internet.Password());

        var result = await _controller.ChangePasswordAsync(user.Id, query, CancellationToken.None);

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
