using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.UserRole;

public class UserRoleRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserRoleRepository _repository;

    public UserRoleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new UserRoleRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetUserRolesAsync_ReturnsPagedUserRoles()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = companyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = companyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = companyId }
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.AddRange(userRoles);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetUserRolesAsync(userId, 1, 2, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUserRolesAsync_WhenUserHasNoRoles_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _repository.GetUserRolesAsync(userId, 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserRolesAsync_OnlyReturnsRolesForActiveCompanies()
    {
        var userId = Guid.NewGuid();
        var activeCompanyId = Guid.NewGuid();
        var inactiveCompanyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var activeCompany = new CompanyModel
        {
            Id = activeCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = inactiveCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = activeCompanyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = inactiveCompanyId }
        };

        _db.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.AddRange(userRoles);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetUserRolesAsync(userId, 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(activeCompanyId, result[0].CompanyId);
    }

    [Fact]
    public async Task CountUserRolesAsync_ReturnsCorrectCount()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = companyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = companyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = companyId }
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.AddRange(userRoles);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountUserRolesAsync(userId, CancellationToken.None);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task CountUserRolesAsync_WhenUserHasNoRoles_ReturnsZero()
    {
        var userId = Guid.NewGuid();

        var result = await _repository.CountUserRolesAsync(userId, CancellationToken.None);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CountUserRolesAsync_OnlyCountsRolesForActiveCompanies()
    {
        var userId = Guid.NewGuid();
        var activeCompanyId = Guid.NewGuid();
        var inactiveCompanyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var activeCompany = new CompanyModel
        {
            Id = activeCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = inactiveCompanyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = activeCompanyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = inactiveCompanyId },
            new() { Id = Guid.NewGuid(), UserId = userId, RoleId = roleId, CompanyId = inactiveCompanyId }
        };

        _db.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.AddRange(userRoles);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountUserRolesAsync(userId, CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetUserRoleAsync_WhenUserHasRoleInCompany_ReturnsUserRole()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(role);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.Add(userRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetUserRoleAsync(userId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userRole.Id, result.Id);
        Assert.Equal(companyId, result.CompanyId);
    }

    [Fact]
    public async Task GetUserRoleAsync_WhenUserHasNoRoleInCompany_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var result = await _repository.GetUserRoleAsync(userId, companyId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task IsAdminAsync_WhenUserIsAdmin_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(adminRole);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.Add(userRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.IsAdminAsync(userId, companyId, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task IsAdminAsync_WhenUserIsNotAdmin_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var userRole = new RoleModel
        {
            Id = userRoleId,
            Name = "User"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRoleMapping = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = userRoleId,
            CompanyId = companyId
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(userRole);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.Add(userRoleMapping);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.IsAdminAsync(userId, companyId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task IsAdminAsync_WhenUserHasNoRoleInCompany_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var result = await _repository.IsAdminAsync(userId, companyId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task IsAdminAsync_WhenRoleNameIsNotExactlyAdmin_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "admin"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = adminRoleId,
            CompanyId = companyId
        };

        _db.AuthCompanies.Add(company);
        _db.AuthRoles.Add(adminRole);
        _db.AuthUsers.Add(user);
        _db.AuthUserRoles.Add(userRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.IsAdminAsync(userId, companyId, CancellationToken.None);

        Assert.False(result);
    }
}
