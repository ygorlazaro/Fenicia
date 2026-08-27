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

    [Fact]
    public async Task Handle_WhenUserIsAdmin_CompanyIsUpdatedSuccessfully()
    {

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

        await handler.Handle(command, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(command.Name, updatedCompany.Name);
        Assert.True(updatedCompany.IsActive);
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
    {

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

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyIsInactive_ThrowsItemNotExistsException()
    {

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

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
    {

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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoRoleInCompany_ThrowsPermissionDeniedException()
    {

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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserHasAdminRoleInDifferentCompany_ThrowsPermissionDeniedException()
    {

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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserHasMultipleRolesIncludingAdmin_CompanyIsUpdated()
    {

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

        await handler.Handle(command, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(command.Name, updatedCompany.Name);
    }

    [Fact]
    public async Task Handle_WhenMultipleAdminsExist_AnyAdminCanUpdate()
    {

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

        await handler.Handle(command, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(command.Name, updatedCompany.Name);
    }

    [Fact]
    public async Task Handle_WhenCompanyExistsButUserHasNoRoles_ThrowsPermissionDeniedException()
    {

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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNameIsNotExactlyAdmin_ThrowsPermissionDeniedException()
    {

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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNameIsAdminWithDifferentCase_ThrowsPermissionDeniedException()
    {

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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesCompanyIsActiveFlagIsPreserved()
    {

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

        await handler.Handle(command, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.True(updatedCompany.IsActive);
        Assert.Equal(company.Cnpj, updatedCompany.Cnpj);
    }

    [Fact]
    public async Task Handle_WhenDatabaseIsEmpty_ThrowsItemNotExistsException()
    {

        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var command = new UpdateCompanyCommand(companyId, userId, faker.Company.CompanyName());

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }
}
