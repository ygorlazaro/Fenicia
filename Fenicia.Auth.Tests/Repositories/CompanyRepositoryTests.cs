namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;
using Domains.Company;

public class CompanyRepositoryTests
{
    private CancellationToken cancellationToken;
    private AuthContext context;
    private Faker faker;
    private DbContextOptions<AuthContext> options;
    private CompanyRepository sut;

    [SetUp]
    public void Setup()
    {
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        var mockLogger = new Mock<ILogger<CompanyRepository>>().Object;

        this.context = new AuthContext(this.options);
        this.sut = new CompanyRepository(this.context, mockLogger);
        this.faker = new Faker();
        this.cancellationToken = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByIDReturnsTrueWhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.CheckCompanyExistsAsync(company.Id, this.cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByIDReturnsFalseWhenNotExists()
    {
        // Act
        var result = await this.sut.CheckCompanyExistsAsync(Guid.NewGuid(), this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByCnpjReturnsTrueWhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.CheckCompanyExistsAsync(company.Cnpj, this.cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AddSavesCompanyToDatabase()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };

        // Act
        this.sut.Add(company);
        await this.sut.SaveAsync(this.cancellationToken);

        // Assert
        var savedCompany = await this.context.Companies.FindAsync([company.Id], this.cancellationToken);
        Assert.That(savedCompany, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedCompany.Name, Is.EqualTo(company.Name));
            Assert.That(savedCompany.Cnpj, Is.EqualTo(company.Cnpj));
        }
    }

    [Test]
    public async Task GetByCnpjAsyncReturnsCompanyWhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetByCnpjAsync(company.Cnpj, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(company.Id));
            Assert.That(result.Cnpj, Is.EqualTo(company.Cnpj));
        }
    }

    [Test]
    public async Task GetByUserIDAsyncReturnsCompaniesWithPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (var i = 0; i < 15; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789")
            };
            companies.Add(company);

            var userRole = new UserRoleModel
            {
                UserId = userId,
                CompanyId = company.Id,
                Company = company
            };
            userRoles.Add(userRole);
        }

        await this.context.Companies.AddRangeAsync(companies, this.cancellationToken);
        await this.context.UserRoles.AddRangeAsync(userRoles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var page1 = await this.sut.GetByUserIdAsync(userId, this.cancellationToken, page: 1, perPage: 10);
        var page2 = await this.sut.GetByUserIdAsync(userId, this.cancellationToken, page: 2, perPage: 10);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(page1, Has.Count.EqualTo(expected: 10));
            Assert.That(page2, Has.Count.EqualTo(expected: 5));
        }
    }

    [Test]
    public async Task CountByUserIDAsyncReturnsCorrectCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedCount = 5;
        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (var i = 0; i < expectedCount; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(length: 14, "0123456789")
            };
            companies.Add(company);

            var userRole = new UserRoleModel
            {
                UserId = userId,
                CompanyId = company.Id,
                Company = company
            };
            userRoles.Add(userRole);
        }

        await this.context.Companies.AddRangeAsync(companies, this.cancellationToken);
        await this.context.UserRoles.AddRangeAsync(userRoles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var count = await this.sut.CountByUserIdAsync(userId, this.cancellationToken);

        // Assert
        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task PatchAsyncUpdatesCompany()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(length: 14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var updatedName = this.faker.Company.CompanyName();
        company.Name = updatedName;

        // Act
        this.sut.PatchAsync(company);
        await this.sut.SaveAsync(this.cancellationToken);

        // Assert
        var updatedCompany = await this.context.Companies.FindAsync([company.Id], this.cancellationToken);
        Assert.That(updatedCompany, Is.Not.Null);
        Assert.That(updatedCompany.Name, Is.EqualTo(updatedName));
    }

    [Test]
    public async Task GetByUserIDAsyncReturnsEmptyListWhenUserHasNoCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await this.sut.GetByUserIdAsync(userId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
