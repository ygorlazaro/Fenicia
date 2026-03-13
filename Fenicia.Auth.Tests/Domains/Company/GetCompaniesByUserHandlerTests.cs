using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.Handlers;
using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

/// <summary>
/// Unit tests for the GetCompaniesByUserHandler.
/// Tests company retrieval logic including pagination, filtering, sorting, and authorization.
/// </summary>
public class GetCompaniesByUserHandlerTests : IDisposable
{
    public GetCompaniesByUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetCompaniesByUserHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly GetCompaniesByUserHandler handler;
    private readonly Faker faker;

    /// <summary>
    /// Tests that a user with no associated companies returns empty pagination.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasNoCompanies_ReturnsEmptyPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(0, result.Pages);
    }

    /// <summary>
    /// Tests that a user with one active company returns it in the pagination.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasOneActiveCompany_ReturnsCompanyInPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Pages);
    }

    /// <summary>
    /// Tests that requesting a page beyond available pages returns empty list.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPageBeyondAvailablePages_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = company.Id
        };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 5, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(5, result.Page);
        Assert.Equal(1, result.Pages);
    }

    /// <summary>
    /// Tests that when a user has multiple roles in the same company, the company appears once per role.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasMultipleRolesInSameCompany_ReturnsCompanyOncePerRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId1 = Guid.NewGuid();
        var roleId2 = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var role1 = new RoleModel
        {
            Id = roleId1,
            Name = "Admin"
        };

        var role2 = new RoleModel
        {
            Id = roleId2,
            Name = "User"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId1,
                CompanyId = companyId
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId2,
                CompanyId = companyId
            }
        };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.AddRange(role1, role2);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.AddRange(userRoles);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Data.Count());
        Assert.Equal(2, result.Total);

        var items = result.Data.ToList();
        Assert.Contains(items, i => i.Role == "Admin");
        Assert.Contains(items, i => i.Role == "User");
    }

    /// <summary>
    /// Tests that results are scoped to the specific user (other users' companies are not returned).
    /// </summary>
    [Fact]
    public async Task Handle_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

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

        var user1 = new UserModel
        {
            Id = userId1,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId1,
            RoleId = roleId,
            CompanyId = company1.Id
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId2,
            RoleId = roleId,
            CompanyId = company2.Id
        };

        this.db.AuthCompanies.AddRange(company1, company2);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.AddRange(user1, user2);
        this.db.AuthUserRoles.AddRange(userRole1, userRole2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId1, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(company1.Name, result.Data.First().Name);
    }

    /// <summary>
    /// Tests that when a user has both active and inactive company associations, only active are returned.
    /// </summary>
    [Fact]
    public async Task Handle_WhenMixedActiveAndInactiveCompanies_ReturnsOnlyActive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = false
        };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var userRoles = new List<UserRoleModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = activeCompany.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = inactiveCompany.Id
            }
        };

        this.db.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.AddRange(userRoles);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(activeCompany.Name, result.Data.First().Name);
    }

    /// <summary>
    /// Tests that zero perPage throws InvalidRequestException.
    /// </summary>
    [Fact]
    public async Task Handle_WithZeroPerPage_ReturnsEmptyData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Internet.UserName(),
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUsers.Add(user);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 0);

        // Act
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await this.handler.Handle(query, CancellationToken.None));
    }
}
