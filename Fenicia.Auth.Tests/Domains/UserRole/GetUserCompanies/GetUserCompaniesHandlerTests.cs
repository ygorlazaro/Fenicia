using Fenicia.Auth.Domains.UserRole.GetUserCompanies;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.UserRole.GetUserCompanies;

[TestFixture]
public class GetUserCompaniesHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new GetUserCompaniesHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private GetUserCompaniesHandler handler = null!;

    [Test]
    public async Task Handle_WhenUserHasCompanies_ReturnsCompanies()
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1), "Should return 1 company");
    }

    [Test]
    public async Task Handle_WhenUserHasNoCompanies_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list");
    }

    [Test]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var companyName = "Test Company";
        var cnpj = "12.345.678/0001-90";
        var roleName = "Admin";

        var company = new CompanyModel
        {
            Id = companyId,
            Name = companyName,
            Cnpj = cnpj,
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.GreaterThan(0), "Should have data");
        var response = result[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Id, Is.EqualTo(companyId), "CompanyId should match");
            Assert.That(response.Role, Is.EqualTo(roleName), "Role should match");
            Assert.That(response.CompanyId, Is.EqualTo(companyId), "Company.Id should match");
            Assert.That(response.CompanyName, Is.EqualTo(companyName), "Company.Name should match");
            Assert.That(response.Cnpj, Is.EqualTo(cnpj), "Company.Cnpj should match");
        }
    }

    [Test]
    public async Task Handle_WhenUserHasMultipleCompanies_ReturnsAllCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        this.context.Roles.Add(role);

        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (var i = 0; i < 3; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = $"Company {i}",
                Cnpj = $"0000000{i}000100",
                IsActive = true,
                TimeZone = "UTC",
                Language = "pt-BR"
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

        this.context.Companies.AddRange(companies);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(3), "Should return all 3 companies");
    }

    [Test]
    public async Task Handle_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
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
        this.context.Roles.Add(role);

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 1",
            Cnpj = "00000001000100",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 2",
            Cnpj = "00000002000100",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.AddRange(company1, company2);

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

        this.context.UserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result1 = await this.handler.Handle(userId1, CancellationToken.None);
        var result2 = await this.handler.Handle(userId2, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Has.Count.EqualTo(1), "Should return only user1's company");
            Assert.That(result1[0].CompanyId, Is.EqualTo(company1.Id), "Should return company1");
            Assert.That(result2, Has.Count.EqualTo(1), "Should return only user2's company");
            Assert.That(result2[0].CompanyId, Is.EqualTo(company2.Id), "Should return company2");
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty list for empty database");
    }

    [Test]
    public async Task Handle_WhenUserHasDifferentRoles_ReturnsAllWithCorrectRoles()
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

        this.context.Roles.AddRange(adminRole, userRole);

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 1",
            Cnpj = "00000001000100",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 2",
            Cnpj = "00000002000100",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.AddRange(company1, company2);

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

        this.context.UserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(2), "Should return 2 companies");
            Assert.That(result.Any(r => r.Role == "Admin"), Is.True, "Should have Admin role");
            Assert.That(result.Any(r => r.Role == "User"), Is.True, "Should have User role");
        }
    }

    [Test]
    public async Task Handle_WhenCompanyHasNullDescription_HandlesCorrectly()
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
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

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1), "Should handle company without issues");
    }
}