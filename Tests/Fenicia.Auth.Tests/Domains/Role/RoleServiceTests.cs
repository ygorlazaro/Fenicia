using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Role;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _mockRepository;
    private readonly RoleService _service;

    public RoleServiceTests()
    {
        _mockRepository = new Mock<IRoleRepository>();
        _service = new RoleService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleExists_ReturnsAdminRole()
    {
        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminRole);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(adminRoleId, result.Id);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleModel?)null);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WhenMultipleRolesExist_ReturnsOnlyAdminRole()
    {
        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminRole);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleNameHasDifferentCase_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleModel?)null);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WithEmptyDatabase_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleModel?)null);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleNameHasExtraSpaces_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoleModel?)null);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WhenMultipleAdminRolesExist_ReturnsFirst()
    {
        var adminRoleId1 = Guid.NewGuid();
        Guid.NewGuid();

        var adminRole1 = new RoleModel
        {
            Id = adminRoleId1,
            Name = "Admin"
        };

        _mockRepository.Setup(r => r.GetByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminRole1);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }
}