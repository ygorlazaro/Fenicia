using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionControllerTests : IDisposable
{
    private readonly SubscriptionController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId = Guid.NewGuid();

    public SubscriptionControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());

        _mockHttpContext = new Mock<HttpContext>();

        var subscriptionRepository = new SubscriptionRepository(_db);
        var userRepository = new UserRepository(_db);
        var userRoleRepository = new UserRoleRepository(_db);
        var roleRepository = new RoleRepository(_db);
        var companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(userRoleRepository);
        var roleService = new RoleService(roleRepository);
        var companyService = new CompanyService(companyRepository, userRoleService);
        var moduleRepository = new ModuleRepository(_db);
        var moduleService = new ModuleService(moduleRepository);
        var userService = new UserService(userRepository, userRoleService, roleService, companyService, new SecurityService(), moduleService);
        var subscriptionService = new SubscriptionService(subscriptionRepository, userService);
        _controller = new SubscriptionController(subscriptionService) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserExists_ReturnsOkWithUserProfile()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = company.Id,
            RoleId = role.Id,
            User = user,
            Company = company,
            Role = role
        };

        var subscription = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Company = company
        };

        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var subscriptionCredit = new SubscriptionCreditModel
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            ModuleId = module.Id,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Module = module
        };

        subscription.Credits = [subscriptionCredit];
        company.UsersRoles = [userRole];
        company.Subscriptions = [subscription];

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthSubscriptions.Add(subscription);
        _db.AuthModules.Add(module);
        _db.AuthSubscriptionCredits.Add(subscriptionCredit);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult);

        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(_testUserId, profile.Id);
        Assert.Equal(user.Email, profile.Email);
        Assert.Equal(user.Name, profile.Name);
        Assert.Single(profile.Companies);
        Assert.Single(profile.Subscriptions);

        var companies = profile.Companies.ToList();
        var subscriptions = profile.Subscriptions.ToList();

        Assert.Equal(company.Id, companies[0].Id);
        Assert.Equal(company.Name, companies[0].Name);
        Assert.Equal(subscription.Id, subscriptions[0].Id);
        Assert.Equal(SubscriptionStatus.Active, subscriptions[0].Status);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var nonExistentUserId = Guid.NewGuid();
        SetupUserClaims(nonExistentUserId);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(nonExistentUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoCompanies_ReturnsOkWithEmptyCompanies()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(_testUserId, profile.Id);
        Assert.Empty(profile.Companies);
        Assert.Empty(profile.Subscriptions);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoSubscriptions_ReturnsOkWithEmptySubscriptions()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = company.Id,
            RoleId = role.Id,
            User = user,
            Company = company,
            Role = role
        };

        company.UsersRoles = [userRole];

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUserRoles.Add(userRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(_testUserId, profile.Id);
        Assert.Single(profile.Companies);
        Assert.Empty(profile.Subscriptions);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasMultipleCompaniesAndSubscriptions_ReturnsOkWithAllData()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = company1.Id,
            RoleId = role.Id,
            User = user,
            Company = company1,
            Role = role
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = company2.Id,
            RoleId = role.Id,
            User = user,
            Company = company2,
            Role = role
        };

        var subscription1 = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            CompanyId = company1.Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Company = company1
        };

        var subscription2 = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            CompanyId = company2.Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Company = company2
        };

        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var subscriptionCredit1 = new SubscriptionCreditModel
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription1.Id,
            ModuleId = module.Id,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30)
        };

        var subscriptionCredit2 = new SubscriptionCreditModel
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription2.Id,
            ModuleId = module.Id,
            IsActive = false,
            StartDate = DateTime.Now.AddDays(-60),
            EndDate = DateTime.Now.AddDays(-30),
            Module = module
        };

        subscription1.Credits = [subscriptionCredit1];
        subscription2.Credits = [subscriptionCredit2];
        company1.UsersRoles = [userRole1];
        company2.UsersRoles = [userRole2];
        company1.Subscriptions = [subscription1];
        company2.Subscriptions = [subscription2];

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company1);
        _db.AuthCompanies.Add(company2);
        _db.AuthUserRoles.Add(userRole1);
        _db.AuthUserRoles.Add(userRole2);
        _db.AuthSubscriptions.Add(subscription1);
        _db.AuthSubscriptions.Add(subscription2);
        _db.AuthModules.Add(module);
        _db.AuthSubscriptionCredits.Add(subscriptionCredit1);
        _db.AuthSubscriptionCredits.Add(subscriptionCredit2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(_testUserId, profile.Id);
        Assert.Equal(2, profile.Companies.Count());
        Assert.Equal(2, profile.Subscriptions.Count());
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
        Assert.NotNull(wide.TraceId);
    }

    [Fact]
    public async Task GetUserProfile_WhenSubscriptionHasInactiveCredits_ReturnsSubscriptionWithOnlyActiveModules()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = company.Id,
            RoleId = role.Id,
            User = user,
            Company = company,
            Role = role
        };

        var subscription = new SubscriptionModel
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Company = company
        };

        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Active Module",
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Module",
            Type = ModuleType.Basic,
            Price = _faker.Finance.Amount(10, 100)
        };

        var activeCredit = new SubscriptionCreditModel
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            ModuleId = module1.Id,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Module = module1
        };

        var inactiveCredit = new SubscriptionCreditModel
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            ModuleId = module2.Id,
            IsActive = false,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(30),
            Module = module2
        };

        subscription.Credits = [activeCredit, inactiveCredit];
        company.UsersRoles = [userRole];
        company.Subscriptions = [subscription];

        _db.AuthUsers.Add(user);
        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUserRoles.Add(userRole);
        _db.AuthSubscriptions.Add(subscription);
        _db.AuthModules.Add(module1);
        _db.AuthModules.Add(module2);
        _db.AuthSubscriptionCredits.Add(activeCredit);
        _db.AuthSubscriptionCredits.Add(inactiveCredit);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Single(profile.Subscriptions);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
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
