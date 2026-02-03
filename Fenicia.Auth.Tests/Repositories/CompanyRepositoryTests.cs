using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

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
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        context = new AuthContext(options);
        sut = new CompanyRepository(context);
        faker = new Faker();
        cancellationToken = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByIdReturnsTrueWhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.CheckCompanyExistsAsync(company.Id, cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByIdReturnsFalseWhenNotExists()
    {
        // Act
        var result = await sut.CheckCompanyExistsAsync(Guid.NewGuid(), cancellationToken);

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
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.CheckCompanyExistsAsync(company.Cnpj, cancellationToken);

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
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };

        // Act
        sut.Add(company);
        await sut.SaveAsync(cancellationToken);

        // Assert
        var savedCompany = await context.Companies.FindAsync([company.Id], cancellationToken);
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
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.GetByCnpjAsync(company.Cnpj, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(company.Id));
            Assert.That(result.Cnpj, Is.EqualTo(company.Cnpj));
        }
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsCompaniesWithPagination()
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
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789")
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

        await context.Companies.AddRangeAsync(companies, cancellationToken);
        await context.UserRoles.AddRangeAsync(userRoles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var page1 = await sut.GetByUserIdAsync(userId, cancellationToken, page: 1, perPage: 10);
        var page2 = await sut.GetByUserIdAsync(userId, cancellationToken, page: 2, perPage: 10);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(page1, Has.Count.EqualTo(expected: 10));
            Assert.That(page2, Has.Count.EqualTo(expected: 5));
        }
    }

    [Test]
    public async Task CountByUserIdAsyncReturnsCorrectCount()
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
                Name = faker.Company.CompanyName(),
                Cnpj = faker.Random.String2(length: 14, "0123456789")
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

        await context.Companies.AddRangeAsync(companies, cancellationToken);
        await context.UserRoles.AddRangeAsync(userRoles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var count = await sut.CountByUserIdAsync(userId, cancellationToken);

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
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var updatedName = faker.Company.CompanyName();
        company.Name = updatedName;

        // Act
        sut.PatchAsync(company);
        await sut.SaveAsync(cancellationToken);

        // Assert
        var updatedCompany = await context.Companies.FindAsync([company.Id], cancellationToken);
        Assert.That(updatedCompany, Is.Not.Null);
        Assert.That(updatedCompany.Name, Is.EqualTo(updatedName));
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsEmptyListWhenUserHasNoCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await sut.GetByUserIdAsync(userId, cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
