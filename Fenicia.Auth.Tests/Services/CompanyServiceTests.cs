using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Exceptions;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class CompanyServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Mock<ICompanyRepository> companyRepositoryMock;
    private Faker faker;
    private CompanyService sut;
    private Mock<IUserRoleService> userRoleServiceMock;

    [SetUp]
    public void Setup()
    {
        companyRepositoryMock = new Mock<ICompanyRepository>();
        userRoleServiceMock = new Mock<IUserRoleService>();
        sut = new CompanyService(companyRepositoryMock.Object, userRoleServiceMock.Object);
        faker = new Faker();
    }

    [Test]
    public void GetByCnpjAsyncWhenRepositoryThrowsThrowsException()
    {
        var cnpj = faker.Random.String2(length: 14, "0123456789");
        companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, onlyActive: false, cancellationToken)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await sut.GetByCnpjAsync(cnpj, cancellationToken));
    }

    [Test]
    public void GetByUserIdAsyncWhenRepositoryThrowsThrowsException()
    {
        var userId = Guid.NewGuid();
        companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, onlyActive: true, cancellationToken, 1, 10)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await sut.GetByUserIdAsync(userId, cancellationToken));
    }

    [Test]
    public async Task PatchAsyncWhenCompanyNotFoundReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken)).ReturnsAsync(false);
        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public async Task PatchAsyncWhenSaveFailsReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken)).ReturnsAsync(true);
        userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", cancellationToken)).ReturnsAsync(true);
        companyRepositoryMock.Setup(x => x.PatchAsync(It.IsAny<CompanyModel>()));
        companyRepositoryMock.Setup(x => x.SaveAsync(cancellationToken)).ReturnsAsync(0);
        // Act & Assert
        Assert.ThrowsAsync<NotSavedException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenRepositoryThrowsThrowsException()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public async Task GetByCnpjAsyncWhenCompanyExistsReturnsCompany()
    {
        // Arrange
        var cnpj = faker.Random.String2(length: 14, "0123456789");
        var companyModel = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
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

        companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, onlyActive: false, cancellationToken)).ReturnsAsync(companyModel);

        // Act
        var result = await sut.GetByCnpjAsync(cnpj, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedResponse.Id));
            Assert.That(result.Name, Is.EqualTo(expectedResponse.Name));
            Assert.That(result.Cnpj, Is.EqualTo(expectedResponse.Cnpj));
            Assert.That(result.Language, Is.EqualTo(expectedResponse.Language));
            Assert.That(result.TimeZone, Is.EqualTo(expectedResponse.TimeZone));
        });
    }

    [Test]
    public async Task GetByCnpjAsyncWhenCompanyDoesNotExistReturnsNotFound()
    {
        // Arrange
        var cnpj = faker.Random.String2(length: 14, "0123456789");

        companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, onlyActive: false, cancellationToken)).ReturnsAsync((CompanyModel)null!);

        // Act
        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut.GetByCnpjAsync(cnpj, cancellationToken));
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789"),
                Language = "pt-BR",
                TimeZone = TimeZoneInfo.Local.StandardName
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789"),
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

        companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, onlyActive: true, cancellationToken, 1, 10)).ReturnsAsync(companies);

        // Act
        var result = await sut.GetByUserIdAsync(userId, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task PatchAsyncWhenCompanyExistsAndUserIsAdminUpdatesCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest { Name = faker.Company.CompanyName() };
        var companyModel = new CompanyModel
        {
            Id = companyId,
            Name = updateRequest.Name,
            Cnpj = faker.Random.String2(length: 14, "0123456789"),
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

        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken)).ReturnsAsync(value: true);

        userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", cancellationToken)).ReturnsAsync(true);

        companyRepositoryMock.Setup(x => x.PatchAsync(It.IsAny<CompanyModel>())).Returns(companyModel);

        companyRepositoryMock.Setup(x => x.SaveAsync(cancellationToken)).ReturnsAsync(value: 1);

        // Act
        var result = await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task PatchAsyncWhenCompanyDoesNotExistReturnsNotFound()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken)).ReturnsAsync(value: false);

        // Act
        // Act & Assert
        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public async Task PatchAsyncWhenUserIsNotAdminReturnsUnauthorized()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken)).ReturnsAsync(value: true);

        userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", cancellationToken)).ReturnsAsync(false);

        // Act
        // Act & Assert
        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public async Task CountByUserIdAsyncReturnsCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedCount = faker.Random.Int(min: 1, max: 100);

        companyRepositoryMock.Setup(x => x.CountByUserIdAsync(userId, onlyActive: true, cancellationToken)).ReturnsAsync(expectedCount);

        // Act
        var result = await sut.CountByUserIdAsync(userId, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
        }
    }
}
