using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Company;

public class CompanyServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<ICompanyRepository> _mockRepository;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly CompanyService _service;

    public CompanyServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<ICompanyRepository>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _service = new CompanyService(_mockRepository.Object, _mockUserRoleService.Object);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenUserHasNoCompanies_ReturnsEmptyPagination()
    {
        var userId = Guid.NewGuid();

        _mockUserRoleService.Setup(s => s.GetUserRolesAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _mockUserRoleService.Setup(s => s.CountUserRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

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
            Name = _faker.Internet.UserName(),
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = companyId,
            Role = role,
            Company = company,
            User = user
        };

        _mockUserRoleService.Setup(s => s.GetUserRolesAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);
        _mockUserRoleService.Setup(s => s.CountUserRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Pages);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WhenPageBeyondAvailablePages_ReturnsEmptyList()
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
            Name = "User"
        };

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Internet.UserName(),
            Password = _faker.Internet.Password()
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CompanyId = company.Id,
            Role = role,
            Company = company,
            User = user
        };

        _mockUserRoleService.Setup(s => s.GetUserRolesAsync(userId, 5, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole]);
        _mockUserRoleService.Setup(s => s.CountUserRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.GetCompaniesByUserAsync(userId, 5, 10, CancellationToken.None);

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
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Internet.UserName(),
            Password = _faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId1,
                CompanyId = companyId,
                Role = role1,
                Company = company,
                User = user
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId2,
                CompanyId = companyId,
                Role = role2,
                Company = company,
                User = user
            }
        };

        _mockUserRoleService.Setup(s => s.GetUserRolesAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRoles);
        _mockUserRoleService.Setup(s => s.CountUserRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var result = await _service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

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
        var roleId = Guid.NewGuid();

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var role = new RoleModel
        {
            Id = roleId,
            Name = "User"
        };

        var user1 = new UserModel
        {
            Id = userId1,
            Email = _faker.Internet.Email(),
            Name = _faker.Internet.UserName(),
            Password = _faker.Internet.Password()
        };

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId1,
            RoleId = roleId,
            CompanyId = company1.Id,
            Role = role,
            Company = company1,
            User = user1
        };

        _mockUserRoleService.Setup(s => s.GetUserRolesAsync(userId1, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userRole1]);
        _mockUserRoleService.Setup(s => s.CountUserRolesAsync(userId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.GetCompaniesByUserAsync(userId1, 1, 10, CancellationToken.None);

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
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
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
            Email = _faker.Internet.Email(),
            Name = _faker.Internet.UserName(),
            Password = _faker.Internet.Password()
        };

        var userRoles = new List<UserRoleModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = activeCompany.Id,
                Role = role,
                Company = activeCompany,
                User = user
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                CompanyId = inactiveCompany.Id,
                Role = role,
                Company = inactiveCompany,
                User = user
            }
        };

        _mockUserRoleService.Setup(s => s.GetUserRolesAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRoles);
        _mockUserRoleService.Setup(s => s.CountUserRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var result = await _service.GetCompaniesByUserAsync(userId, 1, 10, CancellationToken.None);

        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
        Assert.Equal(activeCompany.Name, result.Data.First().Name);
    }

    [Fact]
    public async Task GetCompaniesByUserAsync_WithZeroPerPage_ThrowsInvalidRequestException()
    {
        var userId = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.GetCompaniesByUserAsync(userId, 1, 0, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsAdmin_CompanyIsUpdatedSuccessfully()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var newName = _faker.Company.CompanyName();
        await _service.UpdateAsync(companyId, userId, newName, CancellationToken.None);

        Assert.Equal(newName, company.Name);
        _mockRepository.Verify(r => r.UpdateAsync(company.Id, company, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyDoesNotExist_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompanyModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyIsInactive_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompanyModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsNotAdmin_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
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
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserHasAdminRoleInDifferentCompany_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        Guid.NewGuid();
        var companyId2 = Guid.NewGuid();
        Guid.NewGuid();

        var company2 = new CompanyModel
        {
            Id = companyId2,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company2);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.UpdateAsync(companyId2, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserHasMultipleRolesIncludingAdmin_CompanyIsUpdated()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();
        Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var newName = _faker.Company.CompanyName();
        await _service.UpdateAsync(companyId, userId, newName, CancellationToken.None);

        Assert.Equal(newName, company.Name);
        _mockRepository.Verify(r => r.UpdateAsync(company.Id, company, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMultipleAdminsExist_AnyAdminCanUpdate()
    {
        Guid.NewGuid();
        var admin2Id = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(admin2Id, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var newName = _faker.Company.CompanyName();
        await _service.UpdateAsync(companyId, admin2Id, newName, CancellationToken.None);

        Assert.Equal(newName, company.Name);
        _mockRepository.Verify(r => r.UpdateAsync(company.Id, company, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyExistsButUserHasNoRoles_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNameIsNotExactlyAdmin_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNameIsAdminWithDifferentCase_ThrowsPermissionDeniedException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        Guid.NewGuid();

        var company = new CompanyModel
        {
            Id = companyId,
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _mockUserRoleService.Setup(s => s.IsAdminAsync(userId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("You are not authorized to update this company.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenDatabaseIsEmpty_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.AnyActiveAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompanyModel?)null);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.UpdateAsync(companyId, userId, "Updated Name", CancellationToken.None));
        Assert.Equal("Company not found.", ex.Message);
    }
}
