using System.Net;

using AutoMapper;

using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;

using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class CompanyServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<CompanyService>> _loggerMock;
    private Mock<ICompanyRepository> _companyRepositoryMock;
    private Mock<IUserRoleService> _userRoleServiceMock;
    private CompanyService _sut;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CompanyService>>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _userRoleServiceMock = new Mock<IUserRoleService>();
        _sut = new CompanyService(
            _mapperMock.Object,
            _loggerMock.Object,
            _companyRepositoryMock.Object,
            _userRoleServiceMock.Object
        );
        _faker = new Faker();
    }

    [Test]
    public async Task GetByCnpjAsync_WhenCompanyExists_ReturnsCompany()
    {
        // Arrange
        var cnpj = _faker.Random.String2(14, "0123456789");
        var companyModel = new CompanyModel { Cnpj = cnpj };
        var expectedResponse = new CompanyResponse { Cnpj = cnpj };

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj)).ReturnsAsync(companyModel);

        _mapperMock.Setup(x => x.Map<CompanyResponse>(companyModel)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetByCnpjAsync(cnpj);

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
        var cnpj = _faker.Random.String2(14, "0123456789");

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj)).ReturnsAsync((CompanyModel)null!);

        // Act
        var result = await _sut.GetByCnpjAsync(cnpj);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Message, Is.EqualTo(TextConstants.ItemNotFound));
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

        _companyRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10))
            .ReturnsAsync(companies);

        _mapperMock.Setup(x => x.Map<List<CompanyResponse>>(companies)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetByUserIdAsync(userId);

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

        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId)).ReturnsAsync(true);

        _userRoleServiceMock
            .Setup(x => x.HasRoleAsync(userId, companyId, "Admin"))
            .ReturnsAsync(new ApiResponse<bool>(true));

        _mapperMock.Setup(x => x.Map<CompanyModel>(updateRequest)).Returns(companyModel);

        _companyRepositoryMock
            .Setup(x => x.PatchAsync(It.IsAny<CompanyModel>()))
            .Returns(companyModel);

        _companyRepositoryMock.Setup(x => x.SaveAsync()).ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<CompanyResponse>(companyModel)).Returns(expectedResponse);

        // Act
        var result = await _sut.PatchAsync(companyId, userId, updateRequest);

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

        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId)).ReturnsAsync(false);

        // Act
        var result = await _sut.PatchAsync(companyId, userId, updateRequest);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Message, Is.EqualTo(TextConstants.ItemNotFound));
        });
    }

    [Test]
    public async Task PatchAsync_WhenUserIsNotAdmin_ReturnsUnauthorized()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        _companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId)).ReturnsAsync(true);

        _userRoleServiceMock
            .Setup(x => x.HasRoleAsync(userId, companyId, "Admin"))
            .ReturnsAsync(new ApiResponse<bool>(false));

        // Act
        var result = await _sut.PatchAsync(companyId, userId, updateRequest);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(result.Message, Is.EqualTo(TextConstants.PermissionDenied));
        });
    }

    [Test]
    public async Task CountByUserIdAsync_ReturnsCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedCount = _faker.Random.Int(1, 100);

        _companyRepositoryMock.Setup(x => x.CountByUserIdAsync(userId)).ReturnsAsync(expectedCount);

        // Act
        var result = await _sut.CountByUserIdAsync(userId);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}
