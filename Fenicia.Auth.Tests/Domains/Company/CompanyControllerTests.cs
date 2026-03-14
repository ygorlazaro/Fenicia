using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Company.Commands;
using Fenicia.Auth.Domains.Company.Handlers;
using Fenicia.Auth.Domains.Company.Responses;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Company;

/// <summary>
///     Unit tests for the CompanyController.
///     Tests HTTP endpoints behavior including authorization, pagination, and request/response handling.
/// </summary>
public class CompanyControllerTests : IDisposable
{
    private readonly CompanyController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public CompanyControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.testUserId = Guid.NewGuid();
        var getCompaniesByUserHandler = new GetCompaniesByUserHandler(this.db);
        var updateCompanyHandler = new UpdateCompanyHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new CompanyController(getCompaniesByUserHandler, updateCompanyHandler) { ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object } };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    /// <summary>
    ///     Tests that when a user has no associated companies, the endpoint returns an empty paginated response.
    /// </summary>
    [Fact]
    public async Task GetByLoggedUser_WhenUserHasNoCompanies_ReturnsOkWithEmptyPagination()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetByLoggedUser(query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<IEnumerable<GetCompaniesByUserResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Empty(returnedPagination.Data);
        Assert.Equal(0, returnedPagination.Total);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that when a user has associated companies, the endpoint returns them in a paginated response.
    /// </summary>
    [Fact]
    public async Task GetByLoggedUser_WhenUserHasCompanies_ReturnsOkWithPagination()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel { Id = companyId, Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Company.Cnpj(), IsActive = true };

        var role = new RoleModel { Id = roleId, Name = "Admin" };

        var user = new UserModel { Id = this.testUserId, Email = this.faker.Internet.Email(), Name = this.faker.Person.FullName, Password = this.faker.Internet.Password() };

        var userRole = new UserRoleModel { Id = Guid.NewGuid(), UserId = this.testUserId, RoleId = roleId, CompanyId = companyId };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetByLoggedUser(query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPagination = okResult.Value as Pagination<IEnumerable<GetCompaniesByUserResponse>>;
        Assert.NotNull(returnedPagination);
        Assert.Single(returnedPagination.Data);
        Assert.Equal(1, returnedPagination.Total);
        Assert.Equal(company.Name, returnedPagination.Data.First().Name);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that the WideEventContext UserId is set from the authenticated user claims.
    /// </summary>
    [Fact]
    public async Task GetByLoggedUser_SetsWideEventContextUserId()
    {
        // Arrange
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await this.controller.GetByLoggedUser(query, wide, ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that an Admin user can successfully update a company and receives NoContent result.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenUserIsAdminAndCompanyExists_ReturnsNoContent()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var company = new CompanyModel { Id = companyId, Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Company.Cnpj(), IsActive = true };

        var adminRole = new RoleModel { Id = adminRoleId, Name = "Admin" };

        var user = new UserModel { Id = this.testUserId, Email = this.faker.Internet.Email(), Name = this.faker.Person.FullName, Password = this.faker.Internet.Password() };

        var userRole = new UserRoleModel { Id = Guid.NewGuid(), UserId = this.testUserId, RoleId = adminRoleId, CompanyId = companyId };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(adminRole);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCompanyCommand(companyId, this.testUserId, this.faker.Company.CompanyName());

        // Act
        var result = await this.controller.PatchAsync(companyId, request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);

        // Verify company was updated
        var updatedCompany = await this.db.AuthCompanies.FirstOrDefaultAsync(c => c.Id == companyId, ct);
        Assert.NotNull(updatedCompany);
        Assert.Equal(request.Name, updatedCompany.Name);
    }

    /// <summary>
    ///     Tests that attempting to update a non-existent company throws ItemNotExistsException.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpdateCompanyCommand(companyId, this.testUserId, this.faker.Company.CompanyName());

        // Act & Assert
        await Assert.ThrowsAsync<ItemNotExistsException>(async () => await this.controller.PatchAsync(companyId, request, wide, ct));
    }

    /// <summary>
    ///     Tests that a non-Admin user cannot update a company and receives PermissionDeniedException.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var company = new CompanyModel { Id = companyId, Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Company.Cnpj(), IsActive = true };

        var userRole = new RoleModel { Id = userRoleId, Name = "Contributor" };

        var user = new UserModel { Id = this.testUserId, Email = this.faker.Internet.Email(), Name = this.faker.Person.FullName, Password = this.faker.Internet.Password() };

        var userRoleMapping = new UserRoleModel { Id = Guid.NewGuid(), UserId = this.testUserId, RoleId = userRoleId, CompanyId = companyId };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(userRole);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRoleMapping);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCompanyCommand(companyId, this.testUserId, this.faker.Company.CompanyName());

        // Act & Assert
        await Assert.ThrowsAsync<PermissionDeniedException>(async () => await this.controller.PatchAsync(companyId, request, wide, ct));
    }

    /// <summary>
    ///     Tests that the WideEventContext UserId is set when patching a company.
    /// </summary>
    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var company = new CompanyModel { Id = companyId, Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Company.Cnpj(), IsActive = true };

        var adminRole = new RoleModel { Id = adminRoleId, Name = "Admin" };

        var user = new UserModel { Id = this.testUserId, Email = this.faker.Internet.Email(), Name = this.faker.Person.FullName, Password = this.faker.Internet.Password() };

        var userRole = new UserRoleModel { Id = Guid.NewGuid(), UserId = this.testUserId, RoleId = adminRoleId, CompanyId = companyId };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(adminRole);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCompanyCommand(companyId, this.testUserId, this.faker.Company.CompanyName());

        // Act
        await this.controller.PatchAsync(companyId, request, wide, ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that the CompanyController has the AuthorizeAttribute applied.
    /// </summary>
    [Fact]
    public void CompanyController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(CompanyController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    /// <summary>
    ///     Tests that the CompanyController has the RouteAttribute with correct template.
    /// </summary>
    [Fact]
    public void CompanyController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(CompanyController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    /// <summary>
    ///     Tests that the CompanyController has the ProducesAttribute with correct content type.
    /// </summary>
    [Fact]
    public void CompanyController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(CompanyController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    /// <summary>
    ///     Tests that the PatchAsync method has the AuthorizeAttribute with Admin role.
    /// </summary>
    [Fact]
    public void PatchAsync_HasAuthorizeRolesAttribute()
    {
        // Arrange
        var controllerType = typeof(CompanyController);
        var methodInfo = controllerType.GetMethod(nameof(CompanyController.PatchAsync));

        // Act
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}