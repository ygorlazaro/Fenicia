namespace Fenicia.Auth.Tests.Services;

using System.Net;

using AutoMapper;

using Bogus;

using Common;

using Domains.Company.Data;
using Domains.Company.Logic;
using Domains.LoginAttempt.Logic;
using Domains.Role.Data;
using Domains.Role.Logic;
using Domains.Security.Logic;
using Domains.Token.Data;
using Domains.User.Data;
using Domains.User.Logic;
using Domains.UserRole.Logic;

using Microsoft.Extensions.Logging;

using Moq;

public class UserServiceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> _companyRepositoryMock;
    private Faker _faker;
    private Mock<ILogger<UserService>> _loggerMock;
    private Mock<ILoginAttemptService> _loginAttemptServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IRoleRepository> _roleRepositoryMock;
    private Mock<ISecurityService> _securityServiceMock;
    private UserService _sut;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUserRoleRepository> _userRoleRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _securityServiceMock = new Mock<ISecurityService>();
        _loginAttemptServiceMock = new Mock<ILoginAttemptService>();

        _sut = new UserService(_mapperMock.Object, _loggerMock.Object, _userRepositoryMock.Object, _roleRepositoryMock.Object, _userRoleRepositoryMock.Object, _companyRepositoryMock.Object, _securityServiceMock.Object, _loginAttemptServiceMock.Object);

        _faker = new Faker();
    }

    [Test]
    public async Task GetForLoginAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var request = new TokenRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            Cnpj = _faker.Random.String2(length: 14, chars: "0123456789")
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = "hashedPassword",
            Name = _faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, _cancellationToken)).ReturnsAsync(user);

        _securityServiceMock.Setup(x => x.VerifyPassword(request.Password, user.Password)).Returns(new ApiResponse<bool>(data: true));

        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetForLoginAsync(request, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetForLoginAsync_WithInvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            Cnpj = _faker.Random.String2(length: 14, chars: "0123456789")
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, _cancellationToken)).ReturnsAsync((UserModel)null!);

        // Act
        var result = await _sut.GetForLoginAsync(request, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        });
    }

    [Test]
    public async Task CreateNewUserAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            Name = _faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(length: 14, chars: "0123456789")
            }
        };

        var hashedPassword = "hashedPassword";
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

        _userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, _cancellationToken)).ReturnsAsync(value: false);
        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, _cancellationToken)).ReturnsAsync(value: false);
        _securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(new ApiResponse<string>(hashedPassword));
        _userRepositoryMock.Setup(x => x.Add(It.IsAny<UserModel>())).Returns(user);
        _roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(_cancellationToken)).ReturnsAsync(adminRole);
        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(expectedResponse);

        // Act
        var result = await _sut.CreateNewUserAsync(request, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        _userRepositoryMock.Verify(x => x.SaveAsync(_cancellationToken), Times.Once);
    }

    [Test]
    public async Task CreateNewUserAsync_WithExistingUser_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            Name = _faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(length: 14, chars: "0123456789")
            }
        };

        _userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, _cancellationToken)).ReturnsAsync(value: true);

        // Act
        var result = await _sut.CreateNewUserAsync(request, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result.Data, Is.Null);
        });
    }

    [Test]
    public async Task ExistsInCompanyAsync_ReturnsCorrectResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var exists = true;

        _userRoleRepositoryMock.Setup(x => x.ExistsInCompanyAsync(userId, companyId, _cancellationToken)).ReturnsAsync(exists);

        // Act
        var result = await _sut.ExistsInCompanyAsync(userId, companyId, _cancellationToken);

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
            Email = _faker.Internet.Email(),
            Name = _faker.Name.FullName()
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        _userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId, _cancellationToken)).ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetUserForRefreshAsync(userId, _cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetUserForRefreshAsync_WithInvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId, _cancellationToken)).ReturnsAsync((UserModel)null!);

        // Act
        var result = await _sut.GetUserForRefreshAsync(userId, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(result.Data, Is.Null);
        });
    }

    [Test]
    public async Task CreateNewUserAsync_WithMissingAdminRole_ReturnsInternalServerError()
    {
        // Arrange
        var request = new UserRequest
        {
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password(),
            Name = _faker.Name.FullName(),
            Company = new CompanyRequest
            {
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(length: 14, chars: "0123456789")
            }
        };

        _userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email, _cancellationToken)).ReturnsAsync(value: false);
        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj, _cancellationToken)).ReturnsAsync(value: false);
        _roleRepositoryMock.Setup(x => x.GetAdminRoleAsync(_cancellationToken)).ReturnsAsync((RoleModel)null!);
        _securityServiceMock.Setup(x => x.HashPassword(request.Password)).Returns(new ApiResponse<string>(data: "hashedPassword"));

        // Act
        var result = await _sut.CreateNewUserAsync(request, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Data, Is.Null);
        });
    }
}
