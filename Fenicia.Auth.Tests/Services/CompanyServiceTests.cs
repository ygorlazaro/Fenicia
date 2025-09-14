namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Bogus;

using Common;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;

using Microsoft.Extensions.Logging;

using Moq;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.UserRole;

public class CompanyServiceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> _companyRepositoryMock;
    private Faker _faker;
    private Mock<ILogger<CompanyService>> _loggerMock;
    private CompanyService _sut;
    private Mock<IUserRoleService> _userRoleServiceMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<CompanyService>>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _userRoleServiceMock = new Mock<IUserRoleService>();
        _sut = new CompanyService(_loggerMock.Object, _companyRepositoryMock.Object, _userRoleServiceMock.Object);
        _faker = new Faker();
    }

    [Test]
    public async Task GetByCnpjAsync_WhenCompanyExists_ReturnsCompany()
    {
        // Arrange
        var cnpj = _faker.Random.String2(length: 14, "0123456789");
        var companyModel = new CompanyModel { Cnpj = cnpj };
        var expectedResponse = new CompanyResponse { Cnpj = cnpj };

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, _cancellationToken)).ReturnsAsync(companyModel);

        // Act
        var result = await _sut.GetByCnpjAsync(cnpj, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetByCnpjAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var cnpj = _faker.Random.String2(length: 14, "0123456789");

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, _cancellationToken)).ReturnsAsync((CompanyModel)null!);

        // Act
        var result = await _sut.GetByCnpjAsync(cnpj, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task GetByUserIdAsync_ReturnsCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>
                        {
                            new() { Id = Guid.NewGuid() },
                            new() { Id = Guid.NewGuid() }
                        };
        var expectedResponse = companies.Select(c => new CompanyResponse { Id = c.Id }).ToList();

        _companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, _cancellationToken, 1, 10)).ReturnsAsync(companies);

        // Act
        var result = await _sut.GetByUserIdAsync(userId, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task PatchAsync_WhenCompanyExistsAndUserIsAdmin_UpdatesCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest { Name = _faker.Company.CompanyName() };
        var companyModel = new CompanyModel { Id = companyId, Name = updateRequest.Name };
        var expectedResponse = new CompanyResponse { Id = companyId, Name = updateRequest.Name };

        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, _cancellationToken)).ReturnsAsync(value: true);

        _userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", _cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        _companyRepositoryMock.Setup(x => x.PatchAsync(It.IsAny<CompanyModel>())).Returns(companyModel);

        _companyRepositoryMock.Setup(x => x.SaveAsync(_cancellationToken)).ReturnsAsync(value: 1);

        // Act
        var result = await _sut.PatchAsync(companyId, userId, updateRequest, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task PatchAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, _cancellationToken)).ReturnsAsync(value: false);

        // Act
        var result = await _sut.PatchAsync(companyId, userId, updateRequest, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task PatchAsync_WhenUserIsNotAdmin_ReturnsUnauthorized()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, _cancellationToken)).ReturnsAsync(value: true);

        _userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", _cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: false));

        // Act
        var result = await _sut.PatchAsync(companyId, userId, updateRequest, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
        });
    }

    [Test]
    public async Task CountByUserIdAsync_ReturnsCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedCount = _faker.Random.Int(min: 1, max: 100);

        _companyRepositoryMock.Setup(x => x.CountByUserIdAsync(userId, _cancellationToken)).ReturnsAsync(expectedCount);

        // Act
        var result = await _sut.CountByUserIdAsync(userId, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}
