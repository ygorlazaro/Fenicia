using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.Handlers;
using Fenicia.Auth.Domains.Subscription.Responses;
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
    public SubscriptionControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        var getUserProfileHandler = new GetUserProfileHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new SubscriptionController(getUserProfileHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly SubscriptionController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId = Guid.NewGuid();
    private readonly Faker faker;

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("userId",
                userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetUserProfile_WhenUserExists_ReturnsOkWithUserProfile()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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
            UserId = this.testUserId,
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
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = this.faker.Finance.Amount(10,
                100)
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

        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUserRoles.Add(userRole);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult);

        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(this.testUserId,
            profile.Id);
        Assert.Equal(user.Email,
            profile.Email);
        Assert.Equal(user.Name,
            profile.Name);
        Assert.Single(profile.Companies);
        Assert.Single(profile.Subscriptions);

        var companies = profile.Companies.ToList();
        var subscriptions = profile.Subscriptions.ToList();
        
        Assert.Equal(company.Id,
            companies[0].Id);
        Assert.Equal(company.Name,
            companies[0].Name);
        Assert.Equal(subscription.Id,
            subscriptions[0].Id);
        Assert.Equal(SubscriptionStatus.Active,
            subscriptions[0].Status);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var nonExistentUserId = Guid.NewGuid();
        SetupUserClaims(nonExistentUserId);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(nonExistentUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoCompanies_ReturnsOkWithEmptyCompanies()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(this.testUserId,
            profile.Id);
        Assert.Empty(profile.Companies);
        Assert.Empty(profile.Subscriptions);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoSubscriptions_ReturnsOkWithEmptySubscriptions()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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
            UserId = this.testUserId,
            CompanyId = company.Id,
            RoleId = role.Id,
            User = user,
            Company = company,
            Role = role
        };

        company.UsersRoles = [userRole];

        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(this.testUserId,
            profile.Id);
        Assert.Single(profile.Companies);
        Assert.Empty(profile.Subscriptions);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasMultipleCompaniesAndSubscriptions_ReturnsOkWithAllData()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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
            UserId = this.testUserId,
            CompanyId = company1.Id,
            RoleId = role.Id,
            User = user,
            Company = company1,
            Role = role
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
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
            Status = SubscriptionStatus.Inactive,
            StartDate = DateTime.Now.AddDays(-60),
            EndDate = DateTime.Now.AddDays(-30),
            Company = company2
        };

        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = this.faker.Finance.Amount(10,
                100)
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

        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company1);
        this.db.AuthCompanies.Add(company2);
        this.db.AuthUserRoles.Add(userRole1);
        this.db.AuthUserRoles.Add(userRole2);
        this.db.AuthSubscriptions.Add(subscription1);
        this.db.AuthSubscriptions.Add(subscription2);
        this.db.AuthModules.Add(module);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit1);
        this.db.AuthSubscriptionCredits.Add(subscriptionCredit2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(this.testUserId,
            profile.Id);
        Assert.Equal(2,
            profile.Companies.Count());
        Assert.Equal(2,
            profile.Subscriptions.Count());
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
        Assert.NotNull(wide.TraceId);
    }

    [Fact]
    public async Task GetUserProfile_WhenSubscriptionHasInactiveCredits_ReturnsSubscriptionWithOnlyActiveModules()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
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
            UserId = this.testUserId,
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
            Price = this.faker.Finance.Amount(10,
                100)
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Module",
            Type = ModuleType.Basic,
            Price = this.faker.Finance.Amount(10,
                100)
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

        this.db.AuthUsers.Add(user);
        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUserRoles.Add(userRole);
        this.db.AuthSubscriptions.Add(subscription);
        this.db.AuthModules.Add(module1);
        this.db.AuthModules.Add(module2);
        this.db.AuthSubscriptionCredits.Add(activeCredit);
        this.db.AuthSubscriptionCredits.Add(inactiveCredit);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.controller.GetUserProfile(wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Single(profile.Subscriptions);
        Assert.Equal(this.testUserId.ToString(),
            wide.UserId);
    }
}
