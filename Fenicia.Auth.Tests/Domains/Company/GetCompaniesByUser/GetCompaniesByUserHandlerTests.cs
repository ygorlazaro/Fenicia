using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.GetCompaniesByUser;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company.GetCompaniesByUser;

[TestFixture]
public class GetCompaniesByUserHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new GetCompaniesByUserHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private GetCompaniesByUserHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenUserHasNoCompanies_ReturnsEmptyPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Data, Is.Not.Null, "Data should not be null");
            Assert.That(result.Data.Count(), Is.Zero, "Should return empty list");
            Assert.That(result.Total, Is.Zero, "Total should be zero");
            Assert.That(result.Page, Is.EqualTo(1), "Page should match query");
            Assert.That(result.PerPage, Is.EqualTo(10), "PerPage should match query");
            Assert.That(result.Pages, Is.Zero, "Pages should be zero");
        }
    }

    [Test]
    public async Task Handle_WhenUserHasOneActiveCompany_ReturnsCompanyInPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = this.faker.Company.Cnpj(),
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
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Data.Count(), Is.EqualTo(1), "Should return one company");
            Assert.That(result.Total, Is.EqualTo(1), "Total should be one");
            Assert.That(result.Pages, Is.EqualTo(1), "Should have one page");

            var item = result.Data.First();
            Assert.That(item.Id, Is.EqualTo(companyId), "Company ID should match");
            Assert.That(item.Name, Is.EqualTo("Test Company"), "Company name should match");
            Assert.That(item.Cnpj, Is.EqualTo(company.Cnpj), "CNPJ should match");
            Assert.That(item.Role, Is.EqualTo("Admin"), "Role name should match");
        }
    }

    [Test]
    public async Task Handle_WhenUserHasInactiveCompany_DoesNotReturnInResults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Inactive Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = false,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.Zero, "Should not return inactive companies");
            Assert.That(result.Total, Is.Zero, "Total should be zero");
        }
    }

    [Test]
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
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company2 = new CompanyModel
        {
            Id = companyId2,
            Name = "Alpha Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company3 = new CompanyModel
        {
            Id = companyId3,
            Name = "Beta Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
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

        this.context.Companies.AddRange(company1, company2, company3);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(3), "Should return all three companies");
            Assert.That(result.Total, Is.EqualTo(3), "Total should be three");

            var items = result.Data.ToList();
            Assert.That(items[0].Name, Is.EqualTo("Alpha Company"), "First should be Alpha (sorted)");
            Assert.That(items[1].Name, Is.EqualTo("Beta Company"), "Second should be Beta (sorted)");
            Assert.That(items[2].Name, Is.EqualTo("Zebra Company"), "Third should be Zebra (sorted)");
        }
    }

    [Test]
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
                Name = $"Company {i:D3}",
                Cnpj = this.faker.Company.Cnpj(),
                IsActive = true,
                TimeZone = "UTC",
                Language = "pt-BR"
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
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
        };

        this.context.Companies.AddRange(companies);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 2, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(10), "Should return 10 items per page");
            Assert.That(result.Total, Is.EqualTo(25), "Total should be 25");
            Assert.That(result.Page, Is.EqualTo(2), "Page should be 2");
            Assert.That(result.PerPage, Is.EqualTo(10), "PerPage should be 10");
            Assert.That(result.Pages, Is.EqualTo(3), "Should have 3 pages total");

            var items = result.Data.ToList();
            Assert.That(items[0].Name, Is.EqualTo("Company 010"), "First item on page 2 should be Company 010");
            Assert.That(items[^1].Name, Is.EqualTo("Company 019"), "Last item on page 2 should be Company 019");
        }
    }

    [Test]
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
                Name = $"Company {i:D3}",
                Cnpj = this.faker.Company.Cnpj(),
                IsActive = true,
                TimeZone = "UTC",
                Language = "pt-BR"
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
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
        };

        this.context.Companies.AddRange(companies);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 3, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(5), "Should return remaining 5 items");
            Assert.That(result.Total, Is.EqualTo(25), "Total should be 25");
            Assert.That(result.Page, Is.EqualTo(3), "Page should be 3");
            Assert.That(result.Pages, Is.EqualTo(3), "Should have 3 pages total");

            var items = result.Data.ToList();
            Assert.That(items[0].Name, Is.EqualTo("Company 020"), "First item on page 3 should be Company 020");
            Assert.That(items[^1].Name, Is.EqualTo("Company 024"), "Last item on page 3 should be Company 024");
        }
    }

    [Test]
    public async Task Handle_WhenPageBeyondAvailablePages_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Single Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = company.Id
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 5, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.Zero, "Should return empty list for page beyond available");
            Assert.That(result.Total, Is.EqualTo(1), "Total should still be 1");
            Assert.That(result.Page, Is.EqualTo(5), "Page should be 5 as requested");
            Assert.That(result.Pages, Is.EqualTo(1), "Total pages should be 1");
        }
    }

    [Test]
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
            Name = "Multi-Role Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role1 = new RoleModel
        {
            Id = roleId1,
            Name = "Admin"
        };

        var role2 = new RoleModel
        {
            Id = roleId2,
            Name = "Member"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
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

        this.context.Companies.Add(company);
        this.context.Roles.AddRange(role1, role2);
        this.context.Users.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(2), "Should return company twice (once per role)");
            Assert.That(result.Total, Is.EqualTo(2), "Total should be 2");

            var items = result.Data.ToList();
            Assert.That(items.Any(i => i.Role == "Admin"), Is.True, "Should have Admin role");
            Assert.That(items.Any(i => i.Role == "Member"), Is.True, "Should have Member role");
        }
    }

    [Test]
    public async Task Handle_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "User 1 Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "User 2 Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var user1 = new UserModel
        {
            Id = userId1,
            Email = "user1@example.com",
            Name = "User 1",
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = "user2@example.com",
            Name = "User 2",
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
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

        this.context.Companies.AddRange(company1, company2);
        this.context.Roles.Add(role);
        this.context.Users.AddRange(user1, user2);
        this.context.UserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId1, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(1), "Should return only user 1's company");
            Assert.That(result.Total, Is.EqualTo(1), "Total should be 1");
            Assert.That(result.Data.First().Name, Is.EqualTo("User 1 Company"), "Should return correct company");
        }
    }

    [Test]
    public async Task Handle_WhenMixedActiveAndInactiveCompanies_ReturnsOnlyActive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Active Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = false,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
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

        this.context.Companies.AddRange(activeCompany, inactiveCompany);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(1), "Should return only active company");
            Assert.That(result.Total, Is.EqualTo(1), "Total should be 1");
            Assert.That(result.Data.First().Name, Is.EqualTo("Active Company"), "Should return active company");
        }
    }

    [Test]
    public async Task Handle_WithZeroPerPage_ReturnsEmptyData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = "Test Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Member"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.Users.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 0);

        // Act
        Assert.CatchAsync<ItemNotExistsException>(async () =>
        {
            await this.handler.Handle(query, CancellationToken.None);
        });
    }

    [Test]
    public async Task Handle_WithDifferentRoles_ReturnsCorrectRoleNames()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var companies = new List<CompanyModel>();
        var roles = new List<RoleModel>();
        var userRoles = new List<UserRoleModel>();

        var roleNames = new[] { "Admin", "Manager", "Member", "Viewer" };

        for (var i = 0; i < roleNames.Length; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = $"Company {i + 1}",
                Cnpj = this.faker.Company.Cnpj(),
                IsActive = true,
                TimeZone = "UTC",
                Language = "pt-BR"
            };
            companies.Add(company);

            var role = new RoleModel
            {
                Id = Guid.NewGuid(),
                Name = roleNames[i]
            };
            roles.Add(role);

            var userRole = new UserRoleModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = role.Id,
                CompanyId = company.Id
            };
            userRoles.Add(userRole);
        }

        var user = new UserModel
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            Password = this.faker.Internet.Password()
        };

        this.context.Companies.AddRange(companies);
        this.context.Roles.AddRange(roles);
        this.context.Users.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCompaniesByUserQuery(userId, 1, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Count(), Is.EqualTo(4), "Should return all 4 companies");

            var items = result.Data.ToList();
            Assert.That(items.Select(i => i.Role).OrderBy(r => r), Is.EqualTo(roleNames.OrderBy(r => r)),
                "Should return all role names correctly");
        }
    }
}