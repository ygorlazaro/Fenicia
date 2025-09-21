namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Bogus;

using Common;
using Common.Database.Requests;
using Common.Database.Responses;

using Domains.Company;
using Domains.LoginAttempt;
using Domains.Role;
using Domains.Security;
using Domains.User;
using Domains.UserRole;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.Extensions.Logging;

using Moq;

public class UserServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> companyRepositoryMock;
    private Faker faker;
    private Mock<ILogger<UserService>> loggerMock;
    private Mock<ILoginAttemptService> loginAttemptServiceMock;
    private Mock<IRoleRepository> roleRepositoryMock;
    private Mock<ISecurityService> securityServiceMock;
    private UserService sut;
    private Mock<IUserRepository> userRepositoryMock;
    private Mock<IUserRoleRepository> userRoleRepositoryMock;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<UserService>>();
        this.userRepositoryMock = new Mock<IUserRepository>();
        this.roleRepositoryMock = new Mock<IRoleRepository>();
        this.userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        this.companyRepositoryMock = new Mock<ICompanyRepository>();
        this.securityServiceMock = new Mock<ISecurityService>();
        this.loginAttemptServiceMock = new Mock<ILoginAttemptService>();

        this.sut = new UserService(this.loggerMock.Object, this.userRepositoryMock.Object, this.roleRepositoryMock.Object, this.userRoleRepositoryMock.Object, this.companyRepositoryMock.Object, this.securityServiceMock.Object, this.loginAttemptServiceMock.Object);

        this.faker = new Faker();
    }

    [Test]
    public async Task GetForLoginAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var request = new TokenRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = faker.Internet.Password(),
            Name = this.faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        this.userRepositoryMock.Setup(x => x.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, this.cancellationToken)).ReturnsAsync(user);

        this.securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(new ApiResponse<bool>(data: true));

        // Act
        var result = await this.sut.GetForLoginAsync(request, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetForLoginAsync_WithInvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };

        this.userRepositoryMock.Setup(x => x.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, this.cancellationToken)).ReturnsAsync((UserModel)null!);

        // Act
        var result = await this.sut.GetForLoginAsync(request, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        }
    }

    [Test]
    public async Task CreateNewUserAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789")
            }
        };

        var hashedPassword = this.faker.Internet.Password();
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

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken)).ReturnsAsync(value: false);
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, this.cancellationToken)).ReturnsAsync(value: false);
        this.securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(new ApiResponse<string>(hashedPassword));
        this.userRepositoryMock.Setup(x => x.Add(It.IsAny<UserModel>())).Returns(user);
        this.roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(this.cancellationToken)).ReturnsAsync(adminRole);

        // Act
        var result = await this.sut.CreateNewUserAsync(request, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        this.userRepositoryMock.Verify(x => x.SaveAsync(this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewUserAsync_WithExistingUser_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789")
            }
        };

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await this.sut.CreateNewUserAsync(request, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        }
    }

    [Test]
    public async Task ExistsInCompanyAsync_ReturnsCorrectResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var exists = true;

        this.userRoleRepositoryMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, this.cancellationToken)).ReturnsAsync(exists);

        // Act
        var result = await this.sut.ExistsInCompanyAsync(userId, companyId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(exists));
    }

    [Test]
    public async Task GetUserForRefreshAsync_WithValidUser_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        this.userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId, this.cancellationToken)).ReturnsAsync(user);

        // Act
        var result = await this.sut.GetUserForRefreshAsync(userId, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetUserForRefreshAsync_WithInvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        this.userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId, this.cancellationToken)).ReturnsAsync((UserModel)null!);

        // Act
        var result = await this.sut.GetUserForRefreshAsync(userId, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(result.Data, Is.Null);
        }
    }

    [Test]
    public async Task CreateNewUserAsync_WithMissingAdminRole_ReturnsInternalServerError()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = this.faker.Internet.Email(),
            Password = this.faker.Internet.Password(),
            Name = this.faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789")
            }
        };

        this.userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, this.cancellationToken)).ReturnsAsync(value: false);
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, this.cancellationToken)).ReturnsAsync(value: false);
        this.roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(this.cancellationToken)).ReturnsAsync((RoleModel)null!);
        this.securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(new ApiResponse<string>("hashedPassword"));

        // Act
        var result = await this.sut.CreateNewUserAsync(request, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Data, Is.Null);
        }
    }
}
