using System.Net;

using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class UserServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> companyRepositoryMock;
    private Faker faker;
    private Mock<ILoginAttemptService> loginAttemptServiceMock;
    private Mock<IRoleRepository> roleRepositoryMock;
    private Mock<ISecurityService> securityServiceMock;
    private UserService sut;
    private Mock<IUserRepository> userRepositoryMock;
    private Mock<IUserRoleRepository> userRoleRepositoryMock;

    [SetUp]
    public void Setup()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        roleRepositoryMock = new Mock<IRoleRepository>();
        userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        companyRepositoryMock = new Mock<ICompanyRepository>();
        securityServiceMock = new Mock<ISecurityService>();
        loginAttemptServiceMock = new Mock<ILoginAttemptService>();

        sut = new UserService(userRepositoryMock.Object, roleRepositoryMock.Object, userRoleRepositoryMock.Object, companyRepositoryMock.Object, securityServiceMock.Object, loginAttemptServiceMock.Object);

        faker = new Faker();
    }

    [Test]
    public async Task GetForLoginAsyncWithValidCredentialsReturnsUser()
    {
        // Arrange
        var request = new TokenRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, cancellationToken)).ReturnsAsync(user);

        securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(new ApiResponse<bool>(data: true));

        // Act
        var result = await sut.GetForLoginAsync(request, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetForLoginAsyncWithInvalidUserReturnsBadRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
        };

        userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email, cancellationToken)).ReturnsAsync((UserModel)null!);

        // Act
        var result = await sut.GetForLoginAsync(request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        }
    }

    [Test]
    public async Task CreateNewUserAsyncWithValidDataCreatesUser()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789")
            }
        };

        var hashedPassword = faker.Internet.Password();
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(value: false);
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, cancellationToken)).ReturnsAsync(value: false);
        securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(new ApiResponse<string>(hashedPassword));
        userRepositoryMock.Setup(x => x.Add(It.IsAny<UserModel>())).Returns(user);
        roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(cancellationToken)).ReturnsAsync(adminRole);

        // Act
        var result = await sut.CreateNewUserAsync(request, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        userRepositoryMock.Verify(x => x.SaveAsync(cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewUserAsyncWithExistingUserReturnsBadRequest()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789")
            }
        };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await sut.CreateNewUserAsync(request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        }
    }

    [Test]
    public async Task ExistsInCompanyAsyncReturnsCorrectResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var exists = true;

        userRoleRepositoryMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, cancellationToken)).ReturnsAsync(exists);

        // Act
        var result = await sut.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(exists));
    }

    [Test]
    public async Task GetUserForRefreshAsyncWithValidUserReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId, cancellationToken)).ReturnsAsync(user);

        // Act
        var result = await sut.GetUserForRefreshAsync(userId, cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetUserForRefreshAsyncWithInvalidUserReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId, cancellationToken)).ReturnsAsync((UserModel)null!);

        // Act
        var result = await sut.GetUserForRefreshAsync(userId, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(result.Data, Is.Null);
        }
    }

    [Test]
    public async Task CreateNewUserAsyncWithMissingAdminRoleReturnsInternalServerError()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            Name = faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789")
            }
        };

        userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, cancellationToken)).ReturnsAsync(value: false);
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, cancellationToken)).ReturnsAsync(value: false);
        roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(cancellationToken)).ReturnsAsync((RoleModel)null!);
        securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(new ApiResponse<string>("hashedPassword"));

        // Act
        var result = await sut.CreateNewUserAsync(request, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Data, Is.Null);
        }
    }
}
