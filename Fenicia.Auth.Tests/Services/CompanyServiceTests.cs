using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
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
    private Mock<IRoleService> roleService;

    [SetUp]
    public void Setup()
    {
        companyRepositoryMock = new Mock<ICompanyRepository>();
        userRoleServiceMock = new Mock<IUserRoleService>();
        roleService = new Mock<IRoleService>();
        sut = new CompanyService(companyRepositoryMock.Object, userRoleServiceMock.Object, roleService.Object);
        faker = new Faker();
    }

    [Test]
    public void GetByCnpjAsyncWhenRepositoryThrowsThrowsException()
    {
        var cnpj = faker.Random.String2(14, "0123456789");
        companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, false, cancellationToken)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await sut.GetByCnpjAsync(cnpj, cancellationToken));
    }

    [Test]
    public void GetByUserIdAsyncWhenRepositoryThrowsThrowsException()
    {
        var userId = Guid.NewGuid();
        companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, true, cancellationToken, 1, 10)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await sut.GetByUserIdAsync(userId, cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenCompanyNotFoundReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, cancellationToken)).ReturnsAsync(false);
        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenSaveFailsReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, cancellationToken)).ReturnsAsync(true);
        userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", cancellationToken)).ReturnsAsync(true);
        companyRepositoryMock.Setup(x => x.Update(It.IsAny<CompanyModel>()));
        companyRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(0);
        Assert.ThrowsAsync<NotSavedException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenRepositoryThrowsThrowsException()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, cancellationToken)).ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public async Task GetByCnpjAsyncWhenCompanyExistsReturnsCompany()
    {
        var cnpj = faker.Random.String2(14, "0123456789");
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
            Role = string.Empty
        };

        companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, false, cancellationToken)).ReturnsAsync(companyModel);

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
    public void GetByCnpjAsyncWhenCompanyDoesNotExistReturnsNotFound()
    {
        var cnpj = faker.Random.String2(14, "0123456789");

        companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, false, cancellationToken)).ReturnsAsync((CompanyModel)null!);

        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut.GetByCnpjAsync(cnpj, cancellationToken));
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsCompanies()
    {
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(14, "0123456789"),
                Language = "pt-BR",
                TimeZone = TimeZoneInfo.Local.StandardName
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(14, "0123456789"),
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
            Role = string.Empty
        }).ToList();

        companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, true, cancellationToken, 1, 10)).ReturnsAsync(companies);

        var result = await sut.GetByUserIdAsync(userId, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task PatchAsyncWhenCompanyExistsAndUserIsAdminUpdatesCompany()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest { Name = faker.Company.CompanyName() };
        var companyModel = new CompanyModel
        {
            Id = companyId,
            Name = updateRequest.Name,
            Cnpj = faker.Random.String2(14, "0123456789"),
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
            Role = string.Empty
        };

        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, cancellationToken)).ReturnsAsync(true);

        userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", cancellationToken)).ReturnsAsync(true);

        companyRepositoryMock.Setup(x => x.Update(It.IsAny<CompanyModel>()));

        companyRepositoryMock.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

        var result = await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(expectedResponse.Id));
            Assert.That(result.Name, Is.EqualTo(expectedResponse.Name));
            Assert.That(result.Cnpj, Is.EqualTo(expectedResponse.Cnpj));
            Assert.That(result.Language, Is.EqualTo(expectedResponse.Language));
            Assert.That(result.TimeZone, Is.EqualTo(expectedResponse.TimeZone));
            Assert.That(result.Role, Is.EqualTo(expectedResponse.Role));
        }
    }

    [Test]
    public void PatchAsyncWhenCompanyDoesNotExistReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, cancellationToken)).ReturnsAsync(false);

        Assert.ThrowsAsync<ItemNotExistsException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenUserIsNotAdminReturnsUnauthorized()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, cancellationToken)).ReturnsAsync(true);

        userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", cancellationToken)).ReturnsAsync(false);

        Assert.ThrowsAsync<PermissionDeniedException>(async () => await sut.PatchAsync(companyId, userId, updateRequest, cancellationToken));
    }

    [Test]
    public async Task CountByUserIdAsyncReturnsCount()
    {
        var userId = Guid.NewGuid();
        var expectedCount = faker.Random.Int(1, 100);

        companyRepositoryMock.Setup(x => x.CountByUserIdAsync(userId, true, cancellationToken)).ReturnsAsync(expectedCount);

        var result = await sut.CountByUserIdAsync(userId, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task GetCompaniesAsyncDelegatesToRepository()
    {
        var userId = Guid.NewGuid();
        var expected = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        companyRepositoryMock.Setup(x => x.GetCompaniesAsync(userId, true, cancellationToken)).ReturnsAsync(expected);

        var result = await sut.GetCompaniesAsync(userId, cancellationToken);

        Assert.That(result, Is.EqualTo(expected));
        companyRepositoryMock.Verify(x => x.GetCompaniesAsync(userId, true, cancellationToken), Times.Once);
    }
}
