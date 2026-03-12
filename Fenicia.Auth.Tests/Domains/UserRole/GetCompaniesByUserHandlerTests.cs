using Fenicia.Auth.Domains.UserRole.Handlers;
using Fenicia.Auth.Domains.UserRole.Queries;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.UserRole;

public class GetCompaniesByUserHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetCompaniesByUserHandler handler;

    public GetCompaniesByUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new GetCompaniesByUserHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasCompanies_ReturnsCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = "12345678000190",
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetCompaniesByUserQuery(userId);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasNoCompanies_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new GetCompaniesByUserQuery(userId);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_VerifiesResponseContainsAllFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string companyName = "Test Company";
        const string cnpj = "12.345.678/0001-90";
        const string roleName = "Admin";

        var company = new CompanyModel
        {
            Id = companyId,
            Name = companyName,
            Cnpj = cnpj,
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = roleName
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.db.AuthCompanies.Add(company);
        this.db.AuthRoles.Add(role);
        this.db.AuthUserRoles.Add(userRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetCompaniesByUserQuery(userId);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var response = result[0];
        
        Assert.Equal(companyId,
            response.Id);
        Assert.Equal(roleName,
            response.Role);
        Assert.Equal(companyId,
            response.Company.Id);
        Assert.Equal(companyName,
            response.Company.Name);
        Assert.Equal(cnpj,
            response.Company.Cnpj);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasMultipleCompanies_ReturnsAllCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        this.db.AuthRoles.Add(role);

        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (var i = 0; i < 3; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = $"Company {i}",
                Cnpj = $"0000000{i}000100",
                IsActive = true
            };
            companies.Add(company);

            var userRole = new UserRoleModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyId = company.Id,
                RoleId = roleId
            };
            userRoles.Add(userRole);
        }

        this.db.AuthCompanies.AddRange(companies);
        this.db.AuthUserRoles.AddRange(userRoles);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetCompaniesByUserQuery(userId);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3,
            result.Count);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        this.db.AuthRoles.Add(role);

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 1",
            Cnpj = "00000001000100",
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 2",
            Cnpj = "00000002000100",
            IsActive = true
        };

        this.db.AuthCompanies.AddRange(company1,
            company2);

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId1,
            CompanyId = company1.Id,
            RoleId = roleId
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId2,
            CompanyId = company2.Id,
            RoleId = roleId
        };

        this.db.AuthUserRoles.AddRange(userRole1,
            userRole2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetCompaniesByUserQuery(userId1);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Single(result);
        Assert.Equal(company1.Id,
            result[0].Company.Id);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new GetCompaniesByUserQuery(userId);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasDifferentRoles_ReturnsAllWithCorrectRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "Admin"
        };

        var userRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        this.db.AuthRoles.AddRange(adminRole,
            userRole);

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 1",
            Cnpj = "00000001000100",
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 2",
            Cnpj = "00000002000100",
            IsActive = true
        };

        this.db.AuthCompanies.AddRange(company1,
            company2);

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = company1.Id,
            RoleId = adminRole.Id
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = company2.Id,
            RoleId = userRole.Id
        };

        this.db.AuthUserRoles.AddRange(userRole1,
            userRole2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new GetCompaniesByUserQuery(userId);

        // Act
        var result = await this.handler.GetUserCompaniesAsync(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(2,
            result.Count);
        Assert.Contains(result,
            r => r.Role == "Admin");
        Assert.Contains(result,
            r => r.Role == "User");
    }
}
