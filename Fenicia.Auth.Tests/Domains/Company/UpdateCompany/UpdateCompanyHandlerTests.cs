using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.UpdateCompany;
using Fenicia.Common;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company.UpdateCompany;

[TestFixture]
public class UpdateCompanyHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new UpdateCompanyHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context = null!;
    private UpdateCompanyHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenUserIsAdmin_CompanyIsUpdatedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company Name",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Company Name",
            "America/Sao_Paulo"
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await this.context.Companies.FindAsync(companyId);
        Assert.That(updatedCompany, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCompany!.Name, Is.EqualTo("Updated Company Name"), "Company name should be updated");
            Assert.That(updatedCompany.TimeZone, Is.EqualTo("America/Sao_Paulo"), "TimeZone should be updated");
            Assert.That(updatedCompany.IsActive, Is.True, "IsActive should remain true");
        }
    }

    [Test]
    public async Task Handle_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var nonExistentCompanyId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Existing Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            nonExistentCompanyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.ItemNotFoundMessage));
    }

    [Test]
    public async Task Handle_WhenCompanyIsInactive_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Inactive Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = false,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.ItemNotFoundMessage));
    }

    [Test]
    public async Task Handle_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Contributor"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "Contributor@example.com",
            Name = "Contributor User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.PermissionDeniedMessage));
    }

    [Test]
    public async Task Handle_WhenUserHasNoRoleInCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Admin"
        };

        var otherUser = new AuthUser
        {
            Id = otherUserId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(otherUser);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.PermissionDeniedMessage));
    }

    [Test]
    public async Task Handle_WhenUserHasAdminRoleInDifferentCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId1 = Guid.NewGuid();
        var companyId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company1 = new AuthCompany
        {
            Id = companyId1,
            Name = "Company 1",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company2 = new AuthCompany
        {
            Id = companyId2,
            Name = "Company 2",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId1
        };

        this.context.Companies.AddRange(company1, company2);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId2,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.PermissionDeniedMessage));
    }

    [Test]
    public async Task Handle_WhenUserHasMultipleRolesIncludingAdmin_CompanyIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var memberRoleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var adminRole = new AuthRole
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var memberRole = new AuthRole
        {
            Id = memberRoleId,
            Name = "Contributor"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = this.faker.Internet.Password()
        };

        var userRoles = new List<AuthUserRole>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = adminRoleId,
                CompanyId = companyId
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = memberRoleId,
                CompanyId = companyId
            }
        };

        this.context.Companies.Add(company);
        this.context.Roles.AddRange(adminRole, memberRole);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Company Name",
            "Europe/London"
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await this.context.Companies.FindAsync(companyId);
        Assert.That(updatedCompany, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCompany!.Name, Is.EqualTo("Updated Company Name"), "Company name should be updated");
            Assert.That(updatedCompany.TimeZone, Is.EqualTo("Europe/London"), "TimeZone should be updated");
        }
    }

    [Test]
    public async Task Handle_WhenMultipleAdminsExist_AnyAdminCanUpdate()
    {
        // Arrange
        var admin1Id = Guid.NewGuid();
        var admin2Id = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Admin"
        };

        var admin1 = new AuthUser
        {
            Id = admin1Id,
            Email = "admin1@example.com",
            Name = "Admin 1",
            Password = this.faker.Internet.Password()
        };

        var admin2 = new AuthUser
        {
            Id = admin2Id,
            Email = "admin2@example.com",
            Name = "Admin 2",
            Password = this.faker.Internet.Password()
        };

        var userRoles = new List<AuthUserRole>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = admin1Id,
                RoleId = roleId,
                CompanyId = companyId
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = admin2Id,
                RoleId = roleId,
                CompanyId = companyId
            }
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.AddRange(admin1, admin2);
        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            admin2Id,
            "Updated by Admin 2",
            "Asia/Tokyo"
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await this.context.Companies.FindAsync(companyId);
        Assert.That(updatedCompany, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCompany!.Name, Is.EqualTo("Updated by Admin 2"),
                "Company name should be updated by admin2");
            Assert.That(updatedCompany.TimeZone, Is.EqualTo("Asia/Tokyo"), "TimeZone should be updated");
        }
    }

    [Test]
    public async Task Handle_WhenCompanyExistsButUserHasNoRoles_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Company Without User Roles",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "user@example.com",
            Name = "User",
            Password = this.faker.Internet.Password()
        };

        this.context.Companies.Add(company);
        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.PermissionDeniedMessage));
    }

    [Test]
    public async Task Handle_WhenRoleNameIsNotExactlyAdmin_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Administrator"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "user@example.com",
            Name = "User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.PermissionDeniedMessage));
    }

    [Test]
    public async Task Handle_WhenRoleNameIsAdminWithDifferentCase_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "admin"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "user@example.com",
            Name = "User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.PermissionDeniedMessage));
    }

    [Test]
    public async Task Handle_VerifiesCompanyIsActiveFlagIsPreserved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new AuthCompany
        {
            Id = companyId,
            Name = "Original Company",
            Cnpj = this.faker.Company.Cnpj(),
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var role = new AuthRole
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new AuthUser
        {
            Id = userId,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = this.faker.Internet.Password()
        };

        var userRole = new AuthUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        this.context.Companies.Add(company);
        this.context.Roles.Add(role);
        this.context.AuthUsers.Add(user);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "America/New_York"
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await this.context.Companies.FindAsync(companyId);
        Assert.That(updatedCompany, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCompany!.IsActive, Is.True, "IsActive should remain true after update");
            Assert.That(updatedCompany.Cnpj, Is.EqualTo(company.Cnpj), "CNPJ should not change");
            Assert.That(updatedCompany.Language, Is.EqualTo("pt-BR"), "Language should not change");
        }
    }

    [Test]
    public void Handle_WhenDatabaseIsEmpty_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var command = new UpdateCompanyCommand(
            companyId,
            userId,
            "Updated Name",
            "UTC"
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.ItemNotFoundMessage));
    }
}
