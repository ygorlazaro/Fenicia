using Bogus;

using Fenicia.Auth.Domains.UserRole;

using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class UserRoleServiceTests
{
    private Mock<ILogger<UserRoleService>> _loggerMock;
    private Mock<IUserRoleRepository> _userRoleRepositoryMock;
    private UserRoleService _sut;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<UserRoleService>>();
        _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        _sut = new UserRoleService(_loggerMock.Object, _userRoleRepositoryMock.Object);
        _faker = new Faker();
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasRoles_ReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedRoles = new[] { "Admin", "User", "Manager" };

        _userRoleRepositoryMock
            .Setup(x => x.GetRolesByUserAsync(userId))
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _sut.GetRolesByUserAsync(userId);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedRoles));

        _userRoleRepositoryMock.Verify(x => x.GetRolesByUserAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasNoRoles_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyRoles = Array.Empty<string>();

        _userRoleRepositoryMock.Setup(x => x.GetRolesByUserAsync(userId)).ReturnsAsync(emptyRoles);

        // Act
        var result = await _sut.GetRolesByUserAsync(userId);

        // Assert
        Assert.That(result.Data, Is.Empty);

        _userRoleRepositoryMock.Verify(x => x.GetRolesByUserAsync(userId), Times.Once);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserHasRole_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var role = _faker.Random.ArrayElement(["Admin", "User", "Manager"]);

        _userRoleRepositoryMock
            .Setup(x => x.HasRoleAsync(userId, companyId, role))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, role);

        // Assert
        Assert.That(result.Data, Is.True);

        _userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role), Times.Once);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var role = "NonExistentRole";

        _userRoleRepositoryMock
            .Setup(x => x.HasRoleAsync(userId, companyId, role))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, role);

        // Assert
        Assert.That(result.Data, Is.False);

        _userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role), Times.Once);
    }

    [TestCase("Admin")]
    [TestCase("User")]
    [TestCase("Manager")]
    public async Task HasRoleAsync_WithDifferentRoles_ValidatesCorrectly(string role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        _userRoleRepositoryMock
            .Setup(x => x.HasRoleAsync(userId, companyId, role))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, role);

        // Assert
        Assert.That(result.Data, Is.True);

        _userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role), Times.Once);
    }
}
