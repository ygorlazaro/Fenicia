using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.UserRole;

public class UserRoleServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserRoleService _service;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository companyRepository;

    public UserRoleServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _service = new UserRoleService(_userRoleRepository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasCompanies_ReturnsCompanies()
    {
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

        companyRepository.InsertAsync(company, CancellationToken.None).GetAwaiter().GetResult();
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();
        _userRoleRepository.InsertAsync(userRole, CancellationToken.None).GetAwaiter().GetResult();
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetCompaniesByUserAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasNoCompanies_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _service.GetCompaniesByUserAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_VerifiesResponseContainsAllFields()
    {
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

        companyRepository.InsertAsync(company, CancellationToken.None).GetAwaiter().GetResult();
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();
        _userRoleRepository.InsertAsync(userRole, CancellationToken.None).GetAwaiter().GetResult();
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetCompaniesByUserAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var response = result[0];

        Assert.Equal(companyId, response.Id);
        Assert.Equal(roleName, response.Role);
        Assert.Equal(companyId, response.Company.Id);
        Assert.Equal(companyName, response.Company.Name);
        Assert.Equal(cnpj, response.Company.Cnpj);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasMultipleCompanies_ReturnsAllCompanies()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();

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

        await companyRepository.InsertRangeAsync(companies, CancellationToken.None);
        await _userRoleRepository.InsertRangeAsync(userRoles, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetCompaniesByUserAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();

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

        await companyRepository.InsertRangeAsync(new[] { company1, company2 }, CancellationToken.None);

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

        await _userRoleRepository.InsertRangeAsync(new[] { userRole1, userRole2 }, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetCompaniesByUserAsync(userId1, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Single(result);
        Assert.Equal(company1.Id, result[0].Company.Id);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasDifferentRoles_ReturnsAllWithCorrectRoles()
    {
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

        await _roleRepository.InsertRangeAsync(new[] { adminRole, userRole }, CancellationToken.None);

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

        await companyRepository.InsertRangeAsync(new[] { company1, company2 }, CancellationToken.None);

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

        await _userRoleRepository.InsertRangeAsync(new[] { userRole1, userRole2 }, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetCompaniesByUserAsync(userId, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Role == "Admin");
        Assert.Contains(result, r => r.Role == "User");
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasCompanies_ReturnsCompanies()
    {
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

        companyRepository.InsertAsync(company, CancellationToken.None).GetAwaiter().GetResult();
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();
        _userRoleRepository.InsertAsync(userRole, CancellationToken.None).GetAwaiter().GetResult();
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetUserCompaniesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasNoCompanies_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();

        var result = await _service.GetUserCompaniesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_VerifiesResponseContainsAllFields()
    {
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

        companyRepository.InsertAsync(company, CancellationToken.None).GetAwaiter().GetResult();
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();
        _userRoleRepository.InsertAsync(userRole, CancellationToken.None).GetAwaiter().GetResult();
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetUserCompaniesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var response = result[0];

        Assert.Equal(companyId, response.Id);
        Assert.Equal(roleName, response.Role);
        Assert.Equal(companyId, response.CompanyId);
        Assert.Equal(companyName, response.CompanyName);
        Assert.Equal(cnpj, response.Cnpj);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasMultipleCompanies_ReturnsAllCompanies()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();

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

        await companyRepository.InsertRangeAsync(companies, CancellationToken.None);
        await _userRoleRepository.InsertRangeAsync(userRoles, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetUserCompaniesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenMultipleUsersExist_ReturnsOnlyRequestedUserCompanies()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };
        _roleRepository.InsertAsync(role, CancellationToken.None).GetAwaiter().GetResult();

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

        await companyRepository.InsertRangeAsync(new[] { company1, company2 }, CancellationToken.None);

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

        await _userRoleRepository.InsertRangeAsync(new[] { userRole1, userRole2 }, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result1 = await _service.GetUserCompaniesAsync(userId1, CancellationToken.None);
        var result2 = await _service.GetUserCompaniesAsync(userId2, CancellationToken.None);

        Assert.Single(result1);
        Assert.Equal(company1.Id, result1[0].CompanyId);
        Assert.Single(result2);
        Assert.Equal(company2.Id, result2[0].CompanyId);
    }

    [Fact]
    public async Task GetUserCompaniesAsync_WhenUserHasDifferentRoles_ReturnsAllWithCorrectRoles()
    {
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

        await _roleRepository.InsertRangeAsync(new[] { adminRole, userRole }, CancellationToken.None);

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

        await companyRepository.InsertRangeAsync(new[] { company1, company2 }, CancellationToken.None);

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

        await _userRoleRepository.InsertRangeAsync(new[] { userRole1, userRole2 }, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetUserCompaniesAsync(userId, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Role == "Admin");
        Assert.Contains(result, r => r.Role == "User");
    }
}
