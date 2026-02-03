using Bogus;

using Fenicia.Auth.Domains.UserRole;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class UserRoleServiceTests
{
    private CancellationToken cancellationToken;
    private Faker faker;
    private IUserRoleService sut;
    private Mock<IUserRoleRepository> userRoleRepositoryMock;

    [SetUp]
    public void Setup()
    {
        userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        sut = new IUserRoleService(userRoleRepositoryMock.Object);
        faker = new Faker();
        cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetRolesByUserAsyncWhenUserHasRolesReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedRoles = new[] { "Admin", "User", "Manager" };

        userRoleRepositoryMock.Setup(x => x.GetRolesByUserAsync(userId, cancellationToken)).ReturnsAsync(expectedRoles);

        // Act
        var result = await sut.GetRolesByUserAsync(userId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedRoles));

        userRoleRepositoryMock.Verify(x => x.GetRolesByUserAsync(userId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetRolesByUserAsyncWhenUserHasNoRolesReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyRoles = Array.Empty<string>();

        userRoleRepositoryMock.Setup(x => x.GetRolesByUserAsync(userId, cancellationToken)).ReturnsAsync(emptyRoles);

        // Act
        var result = await sut.GetRolesByUserAsync(userId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Empty);

        userRoleRepositoryMock.Verify(x => x.GetRolesByUserAsync(userId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserHasRoleReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var role = faker.Random.ArrayElement(["Admin", "User", "Manager"]);

        userRoleRepositoryMock.Setup(x => x.HasRoleAsync(userId, companyId, role, cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await sut.HasRoleAsync(userId, companyId, role, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.True);

        userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role, cancellationToken), Times.Once);
    }

    [Test]
    public async Task HasRoleAsyncWhenUserDoesNotHaveRoleReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var role = "NonExistentRole";

        userRoleRepositoryMock.Setup(x => x.HasRoleAsync(userId, companyId, role, cancellationToken)).ReturnsAsync(value: false);

        // Act
        var result = await sut.HasRoleAsync(userId, companyId, role, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.False);

        userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role, cancellationToken), Times.Once);
    }

    [TestCase("Admin")]
    [TestCase("User")]
    [TestCase("Manager")]
    public async Task HasRoleAsyncWithDifferentRolesValidatesCorrectly(string role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        userRoleRepositoryMock.Setup(x => x.HasRoleAsync(userId, companyId, role, cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await sut.HasRoleAsync(userId, companyId, role, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.True);

        userRoleRepositoryMock.Verify(x => x.HasRoleAsync(userId, companyId, role, cancellationToken), Times.Once);
    }
}
