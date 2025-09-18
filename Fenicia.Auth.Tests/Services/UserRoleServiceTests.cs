namespace Fenicia.Auth.Tests.Services;

using Bogus;

using Fenicia.Auth.Domains.UserRole;

using Microsoft.Extensions.Logging;

using Moq;

public class UserRoleServiceTests
{
    private CancellationToken cancellationToken;
    private Faker faker;
    private Mock<ILogger<UserRoleService>> loggerMock;
    private UserRoleService sut;
    private Mock<IUserRoleRepository> userRoleRepositoryMock;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<UserRoleService>>();
        this.userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        this.sut = new UserRoleService(this.loggerMock.Object, this.userRoleRepositoryMock.Object);
        this.faker = new Faker();
        this.cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasRoles_ReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedRoles = new[] { "Admin", "User", "Manager" };

        this.userRoleRepositoryMock.Setup(x => x.GetRolesByUserAsync(userId, this.cancellationToken)).ReturnsAsync(expectedRoles);

        // Act
        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedRoles));

        this.userRoleRepositoryMock.Verify(x => x.GetRolesByUserAsync(userId, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasNoRoles_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyRoles = Array.Empty<string>();

        this.userRoleRepositoryMock.Setup(x => x.GetRolesByUserAsync(userId, this.cancellationToken)).ReturnsAsync(emptyRoles);

        // Act
        var result = await this.sut.GetRolesByUserAsync(userId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        this.userRoleRepositoryMock.Verify(x => x.GetRolesByUserAsync(userId, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserHasRole_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var role = this.faker.Random.ArrayElement(["Admin", "User", "Manager"]);

        this.userRoleRepositoryMock.Setup(x => x.HasRoleAsync(userId, companyId, role, this.cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await this.sut.HasRoleAsync(userId, companyId, role, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.True);

        this.userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var role = "NonExistentRole";

        this.userRoleRepositoryMock.Setup(x => x.HasRoleAsync(userId, companyId, role, this.cancellationToken)).ReturnsAsync(value: false);

        // Act
        var result = await this.sut.HasRoleAsync(userId, companyId, role, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.False);

        this.userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role, this.cancellationToken), Times.Once);
    }

    [TestCase("Admin")]
    [TestCase("User")]
    [TestCase("Manager")]
    public async Task HasRoleAsync_WithDifferentRoles_ValidatesCorrectly(string role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        this.userRoleRepositoryMock.Setup(x => x.HasRoleAsync(userId, companyId, role, this.cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await this.sut.HasRoleAsync(userId, companyId, role, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.True);

        this.userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role, this.cancellationToken), Times.Once);
    }
}
