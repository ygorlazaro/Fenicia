using System.Net;
using AutoMapper;
using Bogus;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fenicia.Auth.Tests.Services;

public class UserServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IRoleRepository> _roleRepositoryMock;
    private Mock<IUserRoleRepository> _userRoleRepositoryMock;
    private Mock<ICompanyRepository> _companyRepositoryMock;
    private Mock<ISecurityService> _securityServiceMock;
    private UserService _sut;
    private Faker _faker;

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

        _sut = new UserService(
            _mapperMock.Object,
            _loggerMock.Object,
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _userRoleRepositoryMock.Object,
            _companyRepositoryMock.Object,
            _securityServiceMock.Object
        );

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
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = "hashedPassword",
            Name = _faker.Name.FullName(),
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAndCnpjAsync(request.Email, request.Cnpj))
            .ReturnsAsync(user);

        _securityServiceMock
            .Setup(x => x.VerifyPassword(request.Password, user.Password))
            .Returns(new ServiceResponse<bool>(true));

        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetForLoginAsync(request);

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
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAndCnpjAsync(request.Email, request.Cnpj))
            .ReturnsAsync((UserModel)null);

        // Act
        var result = await _sut.GetForLoginAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(result.Data, Is.Null);
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
                Cnpj = _faker.Random.String2(14, "0123456789"),
            },
        };

        var hashedPassword = "hashedPassword";
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name,
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
        };

        _userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email)).ReturnsAsync(false);
        _companyRepositoryMock
            .Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj))
            .ReturnsAsync(false);
        _securityServiceMock
            .Setup(x => x.HashPassword(request.Password))
            .Returns(new ServiceResponse<string>(hashedPassword));
        _userRepositoryMock.Setup(x => x.Add(It.IsAny<UserModel>())).Returns(user);
        _roleRepositoryMock.Setup(x => x.GetAdminRoleAsync()).ReturnsAsync(adminRole);
        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(expectedResponse);

        // Act
        var result = await _sut.CreateNewUserAsync(request);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));

        _userRepositoryMock.Verify(x => x.SaveAsync(), Times.Once);
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
                Cnpj = _faker.Random.String2(14, "0123456789"),
            },
        };

        _userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreateNewUserAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(result.Data, Is.Null);
    }

    [Test]
    public async Task ExistsInCompanyAsync_ReturnsCorrectResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var exists = true;

        _userRoleRepositoryMock
            .Setup(x => x.ExistsInCompanyAsync(userId, companyId))
            .ReturnsAsync(exists);

        // Act
        var result = await _sut.ExistsInCompanyAsync(userId, companyId);

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
            Name = _faker.Name.FullName(),
        };

        var expectedResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
        };

        _userRepositoryMock.Setup(x => x.GetUserForRefreshTokenAsync(userId)).ReturnsAsync(user);

        _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetUserForRefreshAsync(userId);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetUserForRefreshAsync_WithInvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetUserForRefreshTokenAsync(userId))
            .ReturnsAsync((UserModel)null);

        // Act
        var result = await _sut.GetUserForRefreshAsync(userId);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(result.Data, Is.Null);
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
                Cnpj = _faker.Random.String2(14, "0123456789"),
            },
        };

        _userRepositoryMock.Setup(x => x.CheckUserExistsAsync(request.Email)).ReturnsAsync(false);
        _companyRepositoryMock
            .Setup(x => x.CheckCompanyExistsAsync(request.Company.Cnpj))
            .ReturnsAsync(false);
        _roleRepositoryMock.Setup(x => x.GetAdminRoleAsync()).ReturnsAsync((RoleModel)null);
        _securityServiceMock
            .Setup(x => x.HashPassword(request.Password))
            .Returns(new ServiceResponse<string>("hashedPassword"));

        // Act
        var result = await _sut.CreateNewUserAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(result.Data, Is.Null);
    }
}
