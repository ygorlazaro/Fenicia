using System.Security.Claims;
using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionControllerTests
{
    private readonly SubscriptionController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly Mock<ISubscriptionService> _mockService;

    public SubscriptionControllerTests()
    {
        _mockHttpContext = new Mock<HttpContext>();
        _mockService = new Mock<ISubscriptionService>();

        _controller = new SubscriptionController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetUserProfile_WhenUserExists_ReturnsOkWithUserProfile()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

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

        var profile = new GetUserProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            [new UserCompanyResponse(company.Id, company.Name, company.Cnpj)],
            [new UserSubscriptionResponse(subscription.Id, company.Id, company.Name, subscription.Status, subscription.StartDate, subscription.EndDate)
            {
                Modules = [new UserModuleResponse(module.Id, module.Name, module.Type)]
            }
        ]);

        _mockService.Setup(s => s.GetUserProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _controller.GetUserProfile(wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult);

        var returnedProfile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(_testUserId, returnedProfile.Id);
        Assert.Equal(user.Email, returnedProfile.Email);
        Assert.Equal(user.Name, returnedProfile.Name);
        Assert.Single(returnedProfile.Companies);
        Assert.Single(returnedProfile.Subscriptions);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.GetUserProfileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetUserProfileResponse?)null);

        var result = await _controller.GetUserProfile(wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUserProfile_WhenUserHasNoSubscriptions_ReturnsOkWithEmptySubscriptions()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

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

        var profile = new GetUserProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            [new UserCompanyResponse(company.Id, company.Name, company.Cnpj)],
            []);

        _mockService.Setup(s => s.GetUserProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _controller.GetUserProfile(wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Equal(_testUserId, returnedProfile.Id);
        Assert.Single(returnedProfile.Companies);
        Assert.Empty(returnedProfile.Subscriptions);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetUserProfile_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = _testUserId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = new SecurityService().Hash(_faker.Internet.Password())
        };

        var profile = new GetUserProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            [],
            []);

        _mockService.Setup(s => s.GetUserProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        await _controller.GetUserProfile(wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
        Assert.NotNull(wide.TraceId);
    }

    [Fact]
    public async Task GetUserProfile_WhenSubscriptionHasInactiveCredits_ReturnsSubscriptionWithOnlyActiveModules()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

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

        var profile = new GetUserProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            [new UserCompanyResponse(company.Id, company.Name, company.Cnpj)],
            [new UserSubscriptionResponse(subscription.Id, company.Id, company.Name, subscription.Status, subscription.StartDate, subscription.EndDate)
            {
                Modules = [new UserModuleResponse(module1.Id, module1.Name, module1.Type)]
            }
        ]);

        _mockService.Setup(s => s.GetUserProfileAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _controller.GetUserProfile(wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProfile = Assert.IsType<GetUserProfileResponse>(okResult.Value);
        Assert.Single(returnedProfile.Subscriptions);
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
