using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.Handlers;
using Fenicia.Auth.Domains.Subscription.Responses;

using MediatR;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionControllerTests : IDisposable
{
    private readonly SubscriptionController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId = Guid.NewGuid();

    public SubscriptionControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());

        var services = new ServiceCollection();
        services.AddSingleton(db);
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetUserProfileHandler>());

        var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ISender>();

        mockHttpContext = new Mock<HttpContext>();

        controller = new SubscriptionController(sender) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetUserProfile_WhenUserExists_ReturnsOkWithUserProfile()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            UserId = testUserId,
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
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = faker.Finance.Amount(10,
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUserRoles.Add(userRole);
        db.AuthSubscriptions.Add(subscription);
        db.AuthModules.Add(module);
        db.AuthSubscriptionCredits.Add(subscriptionCredit);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult);

        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(testUserId, profile.Id);
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
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserDoesNotExist_ReturnsNotFound()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var nonExistentUserId = Guid.NewGuid();
        SetupUserClaims(nonExistentUserId);

        var result = await controller.GetUserProfile(wide, ct);

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
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(testUserId, profile.Id);
        Assert.Empty(profile.Companies);
        Assert.Empty(profile.Subscriptions);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoSubscriptions_ReturnsOkWithEmptySubscriptions()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            UserId = testUserId,
            CompanyId = company.Id,
            RoleId = role.Id,
            User = user,
            Company = company,
            Role = role
        };

        company.UsersRoles = [userRole];

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(testUserId, profile.Id);
        Assert.Single(profile.Companies);
        Assert.Empty(profile.Subscriptions);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasMultipleCompaniesAndSubscriptions_ReturnsOkWithAllData()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            UserId = testUserId,
            CompanyId = company1.Id,
            RoleId = role.Id,
            User = user,
            Company = company1,
            Role = role
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
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
            Name = faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = faker.Finance.Amount(10,
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company1);
        db.AuthCompanies.Add(company2);
        db.AuthUserRoles.Add(userRole1);
        db.AuthUserRoles.Add(userRole2);
        db.AuthSubscriptions.Add(subscription1);
        db.AuthSubscriptions.Add(subscription2);
        db.AuthModules.Add(module);
        db.AuthSubscriptionCredits.Add(subscriptionCredit1);
        db.AuthSubscriptionCredits.Add(subscriptionCredit2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(testUserId, profile.Id);
        Assert.Equal(2, profile.Companies.Count());
        Assert.Equal(2, profile.Subscriptions.Count());
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_SetsWideEventContextUserId()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.Equal(testUserId.ToString(), wide.UserId);
        Assert.NotNull(wide.TraceId);
    }

    [Fact]
    public async Task GetUserProfile_WhenSubscriptionHasInactiveCredits_ReturnsSubscriptionWithOnlyActiveModules()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var user = new UserModel
        {
            Id = testUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            UserId = testUserId,
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
            Price = faker.Finance.Amount(10,
                100)
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Module",
            Type = ModuleType.Basic,
            Price = faker.Finance.Amount(10,
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

        db.AuthUsers.Add(user);
        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUserRoles.Add(userRole);
        db.AuthSubscriptions.Add(subscription);
        db.AuthModules.Add(module1);
        db.AuthModules.Add(module2);
        db.AuthSubscriptionCredits.Add(activeCredit);
        db.AuthSubscriptionCredits.Add(inactiveCredit);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await controller.GetUserProfile(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var profile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Single(profile.Subscriptions);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }
}
