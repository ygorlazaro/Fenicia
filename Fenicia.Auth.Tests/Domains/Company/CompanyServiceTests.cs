using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

public class CompanyServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CompanyService service;

    public CompanyServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        service = new CompanyService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasNoCompanies_ReturnsEmptyPagination()
    {
        var userId = Guid.NewGuid();

        var result = await service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(0, result.Pages);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasOneActiveCompany_ReturnsCompanyInPagination()
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
            Name = faker.Internet.UserName(),
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

        var result = await service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Pages);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenPageBeyondAvailablePages_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Internet.UserName(),
            Password = faker.Internet.Password()
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

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetCompaniesByUserAsync(userId, 5, 10, CancellationToken.None);

        Assert.Empty(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(5, result.Page);
        Assert.Equal(1, result.Pages);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasMultipleRolesInSameCompany_ReturnsCompanyOncePerRole()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId1 = Guid.NewGuid();
        var roleId2 = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
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
            Email = faker.Internet.Email(),
            Name = faker.Internet.UserName(),
            Password = faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel> { new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId1,
            CompanyId = companyId
        }, new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId2,
                CompanyId = companyId
            }
        };

        db.AuthCompanies.Add(company);
        db.AuthRoles.AddRange(role1, role2);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.AddRange(userRoles);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

        Assert.Equal(2, result.Data.Count());
        Assert.Equal(2, result.Total);

        var items = result.Data.ToList();
        Assert.Contains(items, i => i.Role == "Admin");
        Assert.Contains(items, i => i.Role == "User");
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var user1 = new UserModel
        {
            Id = userId1,
            Email = faker.Internet.Email(),
            Name = faker.Internet.UserName(),
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = faker.Internet.Email(),
            Name = faker.Internet.UserName(),
            Password = faker.Internet.Password()
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

        db.AuthCompanies.AddRange(company1, company2);
        db.AuthRoles.Add(role);
        db.AuthUsers.AddRange(user1, user2);
        db.AuthUserRoles.AddRange(userRole1, userRole2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetCompaniesByUserAsync(userId1, 1, 10, CancellationToken.None);

        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(company1.Name, result.Data.First().Name);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenMixedActiveAndInactiveCompanies_ReturnsOnlyActive()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Company.Cnpj(),
            IsActive = false
        };

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Internet.UserName(),
            Password = faker.Internet.Password()
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var userRoles = new List<UserRoleModel> { new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = activeCompany.Id
        }, new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = inactiveCompany.Id
            }
        };

        db.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.AddRange(userRoles);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(activeCompany.Name, result.Data.First().Name);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WithZeroPerPage_ThrowsInvalidRequestException()
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

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Internet.UserName(),
            Password = faker.Internet.Password()
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

        db.AuthCompanies.Add(company);
        db.AuthRoles.Add(role);
        db.AuthUsers.Add(user);
        db.AuthUserRoles.Add(userRole);
        await db.SaveChangesAsync(CancellationToken.None);

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await service.GetCompaniesByUserAsync(userId, 1, 0, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsAdmin_CompanyIsUpdatedSuccessfully()
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

        var newName = faker.Company.CompanyName();
        await service.UpdateAsync(companyId, userId, newName, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(newName, updatedCompany.Name);
        Assert.True(updatedCompany.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
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

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.UpdateAsync(nonExistentCompanyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyIsInactive_ThrowsItemNotExistsException()
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

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserHasNoRoleInCompany_ThrowsPermissionDeniedException()
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserHasAdminRoleInDifferentCompany_ThrowsPermissionDeniedException()
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await service.UpdateAsync(companyId2, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserHasMultipleRolesIncludingAdmin_CompanyIsUpdated()
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

        var newName = faker.Company.CompanyName();
        await service.UpdateAsync(companyId, userId, newName, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(newName, updatedCompany.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenMultipleAdminsExist_AnyAdminCanUpdate()
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

        var newName = faker.Company.CompanyName();
        await service.UpdateAsync(companyId, admin2Id, newName, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.Equal(newName, updatedCompany.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyExistsButUserHasNoRoles_ThrowsPermissionDeniedException()
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNameIsNotExactlyAdmin_ThrowsPermissionDeniedException()
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNameIsAdminWithDifferentCase_ThrowsPermissionDeniedException()
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

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_VerifiesCompanyIsActiveFlagIsPreserved()
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

        var newName = faker.Company.CompanyName();
        await service.UpdateAsync(companyId, userId, newName, CancellationToken.None);

        var updatedCompany = await db.AuthCompanies.FindAsync(companyId);
        Assert.NotNull(updatedCompany);
        Assert.True(updatedCompany.IsActive);
        Assert.Equal(company.Cnpj, updatedCompany.Cnpj);
    }

    [Fact]
    public async Task UpdateAsync_WhenDatabaseIsEmpty_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenCompanyExistsWithMatchingCnpj_ReturnsTrue()
    {
        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenCompanyDoesNotExist_ReturnsFalse()
    {
        var cnpj = faker.Company.Cnpj();

        var result = await service.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsTrueAndCompanyIsActive_ReturnsTrue()
    {
        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.CheckExistsAsync(cnpj, true, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsTrueAndCompanyIsInactive_ReturnsFalse()
    {
        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.CheckExistsAsync(cnpj, true, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsFalseAndCompanyIsInactive_ReturnsTrue()
    {
        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenMultipleCompaniesExist_OnlyMatchesExactCnpj()
    {
        var cnpj1 = faker.Company.Cnpj();
        var cnpj2 = faker.Company.Cnpj();

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj1,
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj2,
            IsActive = true
        };

        db.AuthCompanies.AddRange(company1, company2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.CheckExistsAsync(cnpj1, false, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenMixedActiveAndInactiveCompanies_OnlyActiveFilterWorksCorrectly()
    {
        var cnpj = faker.Company.Cnpj();

        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        db.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        await db.SaveChangesAsync(CancellationToken.None);

        var activeResult = await service.CheckExistsAsync(cnpj, true, CancellationToken.None);
        var inactiveResult = await service.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(activeResult);
        Assert.True(inactiveResult);
    }

    [Fact]
    public async Task CheckExistsAsync_WithEmptyDatabase_ReturnsFalse()
    {
        var result = await service.CheckExistsAsync(faker.Company.Cnpj(), false, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenCnpjContainsSpecialCharacters_NoMatch()
    {
        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = string.Concat(faker.Company.Cnpj(), "./"),
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.False(result);
    }
}
