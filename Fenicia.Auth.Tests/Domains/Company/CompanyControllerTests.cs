using System.Security.Claims;

using Fenicia.Auth.Domains.Company.GetCompaniesByUser;
using Fenicia.Auth.Domains.Company.UpdateCompany;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Company;

[TestFixture]
public class CompanyControllerTests
{
    private Auth.Domains.Company.CompanyController controller = null!;
    private AuthContext context = null!;
    private GetCompaniesByUserHandler getCompaniesByUserHandler = null!;
    private UpdateCompanyHandler updateCompanyHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testUserId;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.testUserId = Guid.NewGuid();
        this.getCompaniesByUserHandler = new GetCompaniesByUserHandler(this.context);
        this.updateCompanyHandler = new UpdateCompanyHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new Auth.Domains.Company.CompanyController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    #region GetByLoggedUser Tests

    [Test]
    public async Task GetByLoggedUser_WhenUserHasNoCompanies_ReturnsOkWithEmptyPagination()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByLoggedUser(
            query,
            this.getCompaniesByUserHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPagination = okResult.Value as Pagination<IEnumerable<CompanyListItemResponse>>;
        Assert.That(returnedPagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPagination!.Data.Count(), Is.Zero);
            Assert.That(returnedPagination.Total, Is.Zero);
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetByLoggedUser_WhenUserHasCompanies_ReturnsOkWithPagination()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByLoggedUser(
            query,
            this.getCompaniesByUserHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPagination = okResult.Value as Pagination<IEnumerable<CompanyListItemResponse>>;
        Assert.That(returnedPagination, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPagination!.Data.Count(), Is.EqualTo(1));
            Assert.That(returnedPagination.Total, Is.EqualTo(1));
            Assert.That(returnedPagination.Data.First().Name, Is.EqualTo("Test Company"));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetByLoggedUser_SetsWideEventContextUserId()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        // Act
        await this.controller.GetByLoggedUser(
            query,
            this.getCompaniesByUserHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    #endregion

    #region PatchAsync Tests

    [Test]
    public async Task PatchAsync_WhenUserIsAdminAndCompanyExists_ReturnsNoContent()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = "hashedPassword"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(adminRole);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCompanyCommand(companyId, this.testUserId, "Updated Company", "America/Sao_Paulo");

        // Act
        var result = await this.controller.PatchAsync(
            companyId,
            request,
            this.updateCompanyHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var noContentResult = result as NoContentResult;
        Assert.That(noContentResult, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }

        // Verify company was updated
        var updatedCompany = await this.context.Companies.FindAsync(companyId);
        Assert.That(updatedCompany, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCompany!.Name, Is.EqualTo("Updated Company"));
            Assert.That(updatedCompany.TimeZone, Is.EqualTo("America/Sao_Paulo"));
        }
    }

    [Test]
    public async Task PatchAsync_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpdateCompanyCommand(companyId, this.testUserId, "Updated Company", "UTC");

        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.controller.PatchAsync(
                companyId,
                request,
                this.updateCompanyHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task PatchAsync_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var userRole = new RoleModel
        {
            Id = userRoleId,
            Name = "Member"
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "member@example.com",
            Name = "Member User",
            Password = "hashedPassword"
        };

        var userRoleMapping = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = userRoleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(userRole);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRoleMapping);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCompanyCommand(companyId, this.testUserId, "Updated Company", "UTC");

        // Act & Assert
        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.controller.PatchAsync(
                companyId,
                request,
                this.updateCompanyHandler,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = "12.345.678/0001-90",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = "hashedPassword"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(adminRole);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCompanyCommand(companyId, this.testUserId, "Updated Company", "UTC");

        // Act
        await this.controller.PatchAsync(
            companyId,
            request,
            this.updateCompanyHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    #endregion

    #region Attribute Tests

    [Test]
    public void CompanyController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Company.CompanyController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "CompanyController should have Authorize attribute");
    }

    [Test]
    public void CompanyController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Company.CompanyController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "CompanyController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void CompanyController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Company.CompanyController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "CompanyController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }

    [Test]
    public void PatchAsync_HasAuthorizeRolesAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Company.CompanyController);
        var methodInfo = controllerType.GetMethod(nameof(Auth.Domains.Company.CompanyController.PatchAsync));

        // Act
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }

    #endregion
}
