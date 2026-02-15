using Bogus;

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
    private Mock<IRoleService> roleService;
    private CompanyService sut;
    private Mock<IUserRoleService> userRoleServiceMock;

    [SetUp]
    public void Setup()
    {
        this.companyRepositoryMock = new Mock<ICompanyRepository>();
        this.userRoleServiceMock = new Mock<IUserRoleService>();
        this.roleService = new Mock<IRoleService>();
        this.sut = new CompanyService(this.companyRepositoryMock.Object, this.userRoleServiceMock.Object,
            this.roleService.Object);
        this.faker = new Faker();
    }

    [Test]
    public void GetByCnpjAsyncWhenRepositoryThrowsThrowsException()
    {
        var cnpj = this.faker.Random.String2(14, "0123456789");
        this.companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, false, this.cancellationToken))
            .ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await this.sut.GetByCnpjAsync(cnpj, this.cancellationToken));
    }

    [Test]
    public void GetByUserIdAsyncWhenRepositoryThrowsThrowsException()
    {
        var userId = Guid.NewGuid();
        this.companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, true, this.cancellationToken, 1, 10))
            .ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () => await this.sut.GetByUserIdAsync(userId, this.cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenCompanyNotFoundReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, this.cancellationToken))
            .ReturnsAsync(false);
        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenSaveFailsReturnsNotFound()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, this.cancellationToken))
            .ReturnsAsync(true);
        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken))
            .ReturnsAsync(true);
        this.companyRepositoryMock.Setup(x => x.Update(It.IsAny<CompanyModel>()));
        this.companyRepositoryMock.Setup(x => x.SaveChangesAsync(this.cancellationToken)).ReturnsAsync(0);
        Assert.ThrowsAsync<NotSavedException>(async () =>
            await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenRepositoryThrowsThrowsException()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();
        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, this.cancellationToken))
            .ThrowsAsync(new Exception("Repo error"));
        Assert.ThrowsAsync<Exception>(async () =>
            await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken));
    }

    [Test]
    public async Task GetByCnpjAsyncWhenCompanyExistsReturnsCompany()
    {
        var cnpj = this.faker.Random.String2(14, "0123456789");
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
            Role = string.Empty
        };

        this.companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, false, this.cancellationToken))
            .ReturnsAsync(companyModel);

        var result = await this.sut.GetByCnpjAsync(cnpj, this.cancellationToken);

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
        var cnpj = this.faker.Random.String2(14, "0123456789");

        this.companyRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj, false, this.cancellationToken))
            .ReturnsAsync((CompanyModel)null!);

        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.sut.GetByCnpjAsync(cnpj, this.cancellationToken));
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsCompanies()
    {
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789"),
                Language = "pt-BR",
                TimeZone = TimeZoneInfo.Local.StandardName
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789"),
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

        this.companyRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, true, this.cancellationToken, 1, 10))
            .ReturnsAsync(companies);

        var result = await this.sut.GetByUserIdAsync(userId, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task PatchAsyncWhenCompanyExistsAndUserIsAdminUpdatesCompany()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest { Name = this.faker.Company.CompanyName() };
        var companyModel = new CompanyModel
        {
            Id = companyId,
            Name = updateRequest.Name,
            Cnpj = this.faker.Random.String2(14, "0123456789"),
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

        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, this.cancellationToken))
            .ReturnsAsync(true);

        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken))
            .ReturnsAsync(true);

        this.companyRepositoryMock.Setup(x => x.Update(It.IsAny<CompanyModel>()));

        this.companyRepositoryMock.Setup(x => x.SaveChangesAsync(this.cancellationToken)).ReturnsAsync(1);

        var result = await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result?.Id, Is.EqualTo(expectedResponse.Id));
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

        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, this.cancellationToken))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken));
    }

    [Test]
    public void PatchAsyncWhenUserIsNotAdminReturnsUnauthorized()
    {
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateRequest = new CompanyUpdateRequest();

        this.companyRepositoryMock.Setup(x => x.CheckCompanyExistsAsync(companyId, true, this.cancellationToken))
            .ReturnsAsync(true);

        this.userRoleServiceMock.Setup(x => x.HasRoleAsync(userId, companyId, "Admin", this.cancellationToken))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.sut.PatchAsync(companyId, userId, updateRequest, this.cancellationToken));
    }

    [Test]
    public async Task CountByUserIdAsyncReturnsCount()
    {
        var userId = Guid.NewGuid();
        var expectedCount = this.faker.Random.Int(1, 100);

        this.companyRepositoryMock.Setup(x => x.CountByUserIdAsync(userId, true, this.cancellationToken))
            .ReturnsAsync(expectedCount);

        var result = await this.sut.CountByUserIdAsync(userId, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task GetCompaniesAsyncDelegatesToRepository()
    {
        var userId = Guid.NewGuid();
        var expected = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        this.companyRepositoryMock.Setup(x => x.GetCompaniesAsync(userId, true, this.cancellationToken))
            .ReturnsAsync(expected);

        var result = await this.sut.GetCompaniesAsync(userId, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expected));
        this.companyRepositoryMock.Verify(x => x.GetCompaniesAsync(userId, true, this.cancellationToken), Times.Once);
    }
}
