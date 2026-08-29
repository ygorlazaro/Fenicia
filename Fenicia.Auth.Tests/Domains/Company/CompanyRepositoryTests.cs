using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

public class CompanyRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CompanyRepository _repository;

    public CompanyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new CompanyRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenCompanyExists_ReturnsCompany()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByCnpjAsync(cnpj, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
        Assert.Equal(cnpj, result.Cnpj);
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByCnpjAsync(_faker.Company.Cnpj(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenCompanyIsDeleted_ReturnsNull()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true,
            Deleted = DateTime.UtcNow
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByCnpjAsync(cnpj, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyActiveAsync_WhenCompanyIsActive_ReturnsCompany()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyActiveAsync(company.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task AnyActiveAsync_WhenCompanyIsInactive_ReturnsNull()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = false
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyActiveAsync(company.Id, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyActiveAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        var result = await _repository.AnyActiveAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyAsync_WhenCompanyExists_ReturnsTrue()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(company.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_WhenCompanyDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.AnyAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WithMatchingCnpjAndActiveCompany_ReturnsTrue()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, true, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WithMatchingCnpjAndOnlyActiveFalse_ReturnsTrue()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WithNoMatch_ReturnsFalse()
    {
        var result = await _repository.CheckExistsAsync(_faker.Company.Cnpj(), false, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsTrueAndCompanyIsInactive_ReturnsFalse()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, true, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsFalseAndCompanyIsInactive_ReturnsTrue()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(result);
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
