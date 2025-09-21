namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Bogus;

using Common;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;

using Microsoft.Extensions.Logging;

using Moq;
using Domains.Company;
using Domains.UserRole;

public class CompanyServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> companyRepositoryMock;
    private Faker faker;
    private Mock<ILogger<CompanyService>> loggerMock;
    private CompanyService sut;
    private Mock<IUserRoleService> userRoleServiceMock;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<CompanyService>>();
        this.companyRepositoryMock = new Mock<ICompanyRepository>();
        this.userRoleServiceMock = new Mock<IUserRoleService>();
        this.sut = new CompanyService(this.loggerMock.Object, this.companyRepositoryMock.Object, this.userRoleServiceMock.Object);
        this.faker = new Faker();
    }

    [Test]
    public void GetByCnpjAsyncWhenRepositoryThrowsThrowsException()
    {
        var cnpj = this.faker.Random.String2(length: 14, "0123456789");
        this.companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, this.cancellationToken)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await this.sut.GetByCnpjAsync(cnpj, this.cancellationToken));
    }

    [Test]
    public void GetByUserIdAsyncWhenRepositoryThrowsThrowsException()
    {
        var userId = Guid.NewGuid();
        this.companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, this.cancellationToken, 1, 10)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await this.sut.GetByUserIdAsync(userId, this.cancellationToken));
    }

    [Test]
    public async Task PatchAsyncWhenCompanyNotFoundReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ReturnsAsync(false);
        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PatchAsyncWhenUserLacksAdminRoleReturnsUnauthorized()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ReturnsAsync(true);
        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(false));
        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task PatchAsyncWhenSaveFailsReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ReturnsAsync(true);
        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(true));
        this.companyRepositoryMock.Setup(x => x.PatchAsync(It.IsAny<CompanyModel>()));
        this.companyRepositoryMock.Setup(x => x.SaveAsync(this.cancellationToken)).ReturnsAsync(0);
        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public void PatchAsyncWhenRepositoryThrowsThrowsException()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken));
    }

    [Test]
    public async Task GetByCnpjAsyncWhenCompanyExistsReturnsCompany()
    {
        // Arrange
        var cnpj = this.faker.Random.String2(length: 14, "0123456789");
        var companyModel = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = cnpj,
            Language = "pt-BR",
            TimeZone = TimeZoneInfo.Local.StandardName
        };
        var expectedResponse = new CompanyResponse
        {
            Id = companyModel.Id,
            Name = companyModel.Name,
            Cnpj = companyModel.Cnpj,
            Language = companyModel.Language,
            TimeZone = companyModel.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        };

        this.companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, this.cancellationToken)).ReturnsAsync(companyModel);

        // Act
        var result = await this.sut.GetByCnpjAsync(cnpj, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test]
    public async Task GetByCnpjAsyncWhenCompanyDoesNotExistReturnsNotFound()
    {
        // Arrange
        var cnpj = this.faker.Random.String2(length: 14, "0123456789");

        this.companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, this.cancellationToken)).ReturnsAsync((CompanyModel)null!);

        // Act
        var result = await this.sut.GetByCnpjAsync(cnpj, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

    [Test]
    public async Task GetByUserIDAsyncReturnsCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789"),
                Language = "pt-BR",
                TimeZone = TimeZoneInfo.Local.StandardName
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789"),
                Language = "pt-BR",
                TimeZone = TimeZoneInfo.Local.StandardName
            }
        };
        var expectedResponse = companies.Select(c => new CompanyResponse
        {
            Id = c.Id,
            Name = c.Name,
            Cnpj = c.Cnpj,
            Language = c.Language,
            TimeZone = c.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        }).ToList();

        this.companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, this.cancellationToken, 1, 10)).ReturnsAsync(companies);

        // Act
        var result = await this.sut.GetByUserIdAsync(userId, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test]
    public async Task PatchAsyncWhenCompanyExistsAndUserIsAdminUpdatesCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest { Name = this.faker.Company.CompanyName() };
        var companyModel = new CompanyModel
        {
            Id = companyId,
            Name = updateRequest.Name,
            Cnpj = this.faker.Random.String2(length: 14, "0123456789"),
            Language = "pt-BR",
            TimeZone = TimeZoneInfo.Local.StandardName
        };
        var expectedResponse = new CompanyResponse
        {
            Id = companyModel.Id,
            Name = companyModel.Name,
            Cnpj = companyModel.Cnpj,
            Language = companyModel.Language,
            TimeZone = companyModel.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        };

        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ReturnsAsync(value: true);

        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: true));

        this.companyRepositoryMock.Setup(x => x.PatchAsync(It.IsAny<CompanyModel>())).Returns(companyModel);

        this.companyRepositoryMock.Setup(x => x.SaveAsync(this.cancellationToken)).ReturnsAsync(value: 1);

        // Act
        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test]
    public async Task PatchAsyncWhenCompanyDoesNotExistReturnsNotFound()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ReturnsAsync(value: false);

        // Act
        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

    [Test]
    public async Task PatchAsyncWhenUserIsNotAdminReturnsUnauthorized()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, this.cancellationToken)).ReturnsAsync(value: true);

        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken)).ReturnsAsync(new ApiResponse<bool>(data: false));

        // Act
        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }

    [Test]
    public async Task CountByUserIDAsyncReturnsCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedCount = this.faker.Random.Int(min: 1, max: 100);

        this.companyRepositoryMock.Setup(x => x.CountByUserIdAsync(userId, this.cancellationToken)).ReturnsAsync(expectedCount);

        // Act
        var result = await this.sut.CountByUserIdAsync(userId, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
