using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.GetCompaniesByUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

public class GetCompaniesByUserHandlerTests : IDisposable
{
    public GetCompaniesByUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetCompaniesByUserHandler(this.context);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly GetCompaniesByUserHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenUserHasNoCompanies_ReturnsEmptyPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(0, result.Pages);
    }

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

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Pages);

        var item = result.Data.First();
        Assert.Equal(companyId, item.Id);
        Assert.Equal(company.Name, item.Name);
        Assert.Equal(company.Cnpj, item.Cnpj);
        Assert.Equal("Admin", item.Role);
    }

    [Fact]
    public async Task Handle_WhenUserHasInactiveCompany_DoesNotReturnInResults()
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
            IsActive = false
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
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

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_WhenUserHasMultipleCompanies_ReturnsAllSortedByName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId1 = Guid.NewGuid();
        var companyId2 = Guid.NewGuid();
        var companyId3 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company1 = new CompanyModel
        {
            Id = companyId1,
            Name = "Zebra Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = companyId2,
            Name = "Beta Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var company3 = new CompanyModel
        {
            Id = companyId3,
            Name = "Alpha Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Contributor"
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
                RoleId = roleId,
                CompanyId = companyId1
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = companyId2
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = companyId3
            }
        };

        this.context.AuthCompanies.AddRange(company1, company2, company3);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Data.Count());
        Assert.Equal(3, result.Total);

        var items = result.Data.ToList();
        Assert.Equal("Alpha Company", items[0].Name);
        Assert.Equal("Beta Company", items[1].Name);
        Assert.Equal("Zebra Company", items[2].Name);
    }

    [Fact]
    public async Task Handle_WhenPaginationRequested_ReturnsCorrectPage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (var i = 0; i < 25; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Company.Cnpj(),
                IsActive = true
            };
            companies.Add(company);

            var userRole = new UserRoleModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = company.Id
            };
            userRoles.Add(userRole);
        }

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
            Name = "Contributor"
        };

        this.context.AuthCompanies.AddRange(companies);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 2, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(10, result.Data.Count());
        Assert.Equal(25, result.Total);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(3, result.Pages);

        var items = result.Data.ToList();
        var sortedCompanies = companies.OrderBy(c => c.Name).ToList();
        Assert.Equal(sortedCompanies.Skip(10).FirstOrDefault()?.Name, items[0].Name);
        Assert.Equal(sortedCompanies.Skip(10).Take(10).LastOrDefault()?.Name, items[^1].Name);
    }

    [Fact]
    public async Task Handle_WhenLastPageRequested_ReturnsRemainingItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (var i = 0; i < 25; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Company.Cnpj(),
                IsActive = true
            };

            companies.Add(company);

            var userRole = new UserRoleModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = company.Id
            };
            userRoles.Add(userRole);
        }

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
            Name = "Contributor"
        };

        this.context.AuthCompanies.AddRange(companies);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 3, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Data.Count());
        Assert.Equal(25, result.Total);
        Assert.Equal(3, result.Page);
        Assert.Equal(3, result.Pages);

        var items = result.Data.ToList();
        var sortedCompanies = companies.OrderBy(c => c.Name).ToList();
        Assert.Equal(sortedCompanies.Skip(20).FirstOrDefault()?.Name, items[0].Name);
        Assert.Equal(sortedCompanies.Skip(20).LastOrDefault()?.Name, items[^1].Name);
    }

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
            Name = "Contributor"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = company.Id
        };

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 5, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(5, result.Page);
        Assert.Equal(1, result.Pages);
    }

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
            Name = "Contributor"
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

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.AddRange(role1, role2);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Data.Count());
        Assert.Equal(2, result.Total);

        var items = result.Data.ToList();
        Assert.Contains(items, i => i.Role == "Admin");
        Assert.Contains(items, i => i.Role == "Contributor");
    }

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
            Name = "Contributor"
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

        this.context.AuthCompanies.AddRange(company1, company2);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.AddRange(user1, user2);
        this.context.AuthUserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId1, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(company1.Name, result.Data.First().Name);
    }

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
            Name = "Contributor"
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

        this.context.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(activeCompany.Name, result.Data.First().Name);
    }

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
            Name = "Contributor"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.AuthCompanies.Add(company);
        this.context.AuthRoles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.AuthUserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 0);

        // Act
        await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
    }
}
