using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.Commands;
using Fenicia.Auth.Domains.Company.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

/// <summary>
///     Unit tests for the UpdateCompanyHandler.
///     Tests business logic for updating company information including authorization, validation, and data integrity.
/// </summary>
public class UpdateCompanyHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateCompanyHandler handler;

    public UpdateCompanyHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new UpdateCompanyHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that an Admin user can successfully update a company's name.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIsAdmin_CompanyIsUpdatedSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            Email = faker.Internet.Email(),
            Name = faker.Name.FullName(),
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(command.Name, updatedCompany.Name);
        Assert.True(updatedCompany.IsActive);
    }

    /// <summary>
    ///     Tests that updating a non-existent company throws ItemNotExistsException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var nonExistentCompanyId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(nonExistentCompanyId, userId, "Updated Name");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    /// <summary>
    ///     Tests that updating an inactive company throws ItemNotExistsException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCompanyIsInactive_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = false
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FirstName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);
        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    /// <summary>
    ///     Tests that a non-Admin user cannot update a company.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    /// <summary>
    ///     Tests that a user without any role in the company cannot update it.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasNoRoleInCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var otherUser = new UserModel
        {
            Id = otherUserId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(otherUser);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    /// <summary>
    ///     Tests that a user with Admin role in one company cannot update a different company.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasAdminRoleInDifferentCompany_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId1 = Guid.NewGuid();
        var companyId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company1 = new CompanyModel
        {
            Id = companyId1,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = companyId2,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId1
        };

        db.AuthCompanies.AddRange(company1, company2);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId2, userId, faker.Company.CompanyName());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    /// <summary>
    ///     Tests that a user with multiple roles including Admin can update the company.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasMultipleRolesIncludingAdmin_CompanyIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var memberRoleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var memberRole = new RoleModel
        {
            Id = memberRoleId,
            Name = "User"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel> { new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = adminRoleId,
            CompanyId = companyId
        }, new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = memberRoleId,
                CompanyId = companyId
            }
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.AddRange(adminRole, memberRole);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.AddRange(userRoles);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(command.Name, updatedCompany.Name);
    }

    /// <summary>
    ///     Tests that any Admin user can update a company when multiple Admins exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenMultipleAdminsExist_AnyAdminCanUpdate()
    {
        // Arrange
        var admin1Id = Guid.NewGuid();
        var admin2Id = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var admin1 = new UserModel
        {
            Id = admin1Id,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var admin2 = new UserModel
        {
            Id = admin2Id,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel> { new()
        {
            Id = Guid.NewGuid(),
            UserId = admin1Id,
            RoleId = roleId,
            CompanyId = companyId
        }, new()
            {
                Id = Guid.NewGuid(),
                UserId = admin2Id,
                RoleId = roleId,
                CompanyId = companyId
            }
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.AddRange(admin1, admin2);
        db.AuthUserRoles.AddRange(userRoles);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, admin2Id, faker.Company.CompanyName());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(command.Name, updatedCompany.Name);
    }

    /// <summary>
    ///     Tests that a user without any role in the company cannot update it.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCompanyExistsButUserHasNoRoles_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthCompanies.Add(company);
        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, "Updated Name");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    /// <summary>
    ///     Tests that the handler requires exact "Admin" role name for authorization.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleNameIsNotExactlyAdmin_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, "Updated Name");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    /// <summary>
    ///     Tests that role name matching is case-sensitive (admin != Admin).
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleNameIsAdminWithDifferentCase_ThrowsPermissionDeniedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, "Updated Name");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    /// <summary>
    ///     Tests that updating a company preserves the IsActive flag and CNPJ.
    /// </summary>
    [Fact]
    public async Task Handle_VerifiesCompanyIsActiveFlagIsPreserved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.True(updatedCompany.IsActive);
        Assert.Equal(company.Cnpj, updatedCompany.Cnpj);
    }

    /// <summary>
    ///     Tests that updating a company when no companies exist throws ItemNotExistsException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenDatabaseIsEmpty_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }
}
