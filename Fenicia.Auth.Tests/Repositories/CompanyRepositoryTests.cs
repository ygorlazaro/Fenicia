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
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.CheckCompanyExistsAsync(company.Id, onlyActive: true, cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByIdReturnsFalseWhenNotExists()
    {
        var result = await sut.CheckCompanyExistsAsync(Guid.NewGuid(), onlyActive: true, cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByCnpjReturnsTrueWhenExists()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.CheckCompanyExistsAsync(company.Cnpj, onlyActive: true, cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AddSavesCompanyToDatabase()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };

        sut.Add(company);
        await sut.SaveChangesAsync(cancellationToken);

        var savedCompany = await context.Companies.FindAsync([company.Id], cancellationToken);
        Assert.That(savedCompany, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(savedCompany.Name, Is.EqualTo(company.Name));
            Assert.That(savedCompany.Cnpj, Is.EqualTo(company.Cnpj));
        });
    }

    [Test]
    public async Task GetByCnpjAsyncReturnsCompanyWhenExists()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789")
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetByCnpjAsync(company.Cnpj, onlyActive: true, cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(company.Id));
            Assert.That(result.Cnpj, Is.EqualTo(company.Cnpj));
        });
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsCompaniesWithPagination()
    {
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

        var page1 = await sut.GetByUserIdAsync(userId, onlyActive: true, cancellationToken, page: 1, perPage: 10);
        var page2 = await sut.GetByUserIdAsync(userId, onlyActive: true, cancellationToken, page: 2, perPage: 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page1, Has.Count.EqualTo(expected: 10));
            Assert.That(page2, Has.Count.EqualTo(expected: 5));
        }
    }

    [Test]
    public async Task CountByUserIdAsyncReturnsCorrectCount()
    {
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

        var count = await sut.CountByUserIdAsync(userId, onlyActive: true, cancellationToken);

        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task PatchAsyncUpdatesCompany()
    {
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

        sut.Update(company);
        await sut.SaveChangesAsync(cancellationToken);

        var updatedCompany = await context.Companies.FindAsync([company.Id], cancellationToken);
        Assert.That(updatedCompany, Is.Not.Null);
        Assert.That(updatedCompany.Name, Is.EqualTo(updatedName));
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsEmptyListWhenUserHasNoCompanies()
    {
        var userId = Guid.NewGuid();

        var result = await sut.GetByUserIdAsync(userId, onlyActive: true, cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CheckCompanyExistsRespectsOnlyActiveFlagByIdAndCnpj()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789"),
            IsActive = false
        };
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var existsOnlyActiveById = await sut.CheckCompanyExistsAsync(company.Id, onlyActive: true, cancellationToken);
        var existsAnyById = await sut.CheckCompanyExistsAsync(company.Id, onlyActive: false, cancellationToken);

        var existsOnlyActiveByCnpj = await sut.CheckCompanyExistsAsync(company.Cnpj, onlyActive: true, cancellationToken);
        var existsAnyByCnpj = await sut.CheckCompanyExistsAsync(company.Cnpj, onlyActive: false, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(existsOnlyActiveById, Is.False);
            Assert.That(existsAnyById, Is.True);
            Assert.That(existsOnlyActiveByCnpj, Is.False);
            Assert.That(existsAnyByCnpj, Is.True);
        });
    }

    [Test]
    public async Task GetByCnpjAsyncRespectsOnlyActiveFlag()
    {
        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = faker.Random.String2(length: 14, "0123456789"),
            IsActive = true
        };
        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = activeCompany.Cnpj,
            IsActive = false
        };

        await context.Companies.AddAsync(inactiveCompany, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var resultOnlyActive = await sut.GetByCnpjAsync(activeCompany.Cnpj, onlyActive: true, cancellationToken);
        var resultAny = await sut.GetByCnpjAsync(activeCompany.Cnpj, onlyActive: false, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(resultOnlyActive, Is.Null);
            Assert.That(resultAny, Is.Not.Null);
        });
    }

    [Test]
    public async Task GetByUserIdAsyncIncludesInactiveWhenOnlyActiveFalse()
    {
        var userId = Guid.NewGuid();
        var activeCompany = new CompanyModel { Id = Guid.NewGuid(), Name = faker.Company.CompanyName(), Cnpj = faker.Random.String2(14, "0123456789"), IsActive = true };
        var inactiveCompany = new CompanyModel { Id = Guid.NewGuid(), Name = faker.Company.CompanyName(), Cnpj = faker.Random.String2(14, "0123456789"), IsActive = false };

        var userRoles = new List<UserRoleModel>
        {
            new() { UserId = userId, CompanyId = activeCompany.Id, Company = activeCompany },
            new() { UserId = userId, CompanyId = inactiveCompany.Id, Company = inactiveCompany }
        };

        await context.Companies.AddRangeAsync([activeCompany, inactiveCompany], cancellationToken);
        await context.UserRoles.AddRangeAsync(userRoles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var onlyActive = await sut.GetByUserIdAsync(userId, onlyActive: true, cancellationToken);
        var any = await sut.GetByUserIdAsync(userId, onlyActive: false, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(onlyActive.Select(c => c.Id), Does.Not.Contain(inactiveCompany.Id));
            Assert.That(any.Select(c => c.Id), Does.Contain(inactiveCompany.Id));
        });
    }

    [Test]
    public async Task GetCompaniesAsyncReturnsDistinctIdsAndRespectsOnlyActive()
    {
        var userId = Guid.NewGuid();
        var activeCompany = new CompanyModel { Id = Guid.NewGuid(), Name = faker.Company.CompanyName(), Cnpj = faker.Random.String2(14, "0123456789"), IsActive = true };
        var inactiveCompany = new CompanyModel { Id = Guid.NewGuid(), Name = faker.Company.CompanyName(), Cnpj = faker.Random.String2(14, "0123456789"), IsActive = false };

        var userRoles = new List<UserRoleModel>
        {
            new() { UserId = userId, CompanyId = activeCompany.Id },
            new() { UserId = userId, CompanyId = activeCompany.Id },
            new() { UserId = userId, CompanyId = inactiveCompany.Id }
        };

        await context.Companies.AddRangeAsync([activeCompany, inactiveCompany], cancellationToken);
        await context.UserRoles.AddRangeAsync(userRoles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var onlyActive = await sut.GetCompaniesAsync(userId, onlyActive: true, cancellationToken);
        var any = await sut.GetCompaniesAsync(userId, onlyActive: false, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(onlyActive, Has.Count.EqualTo(1));
            Assert.That(onlyActive, Does.Contain(activeCompany.Id));
            Assert.That(any, Has.Count.EqualTo(2));
            Assert.That(any, Does.Contain(inactiveCompany.Id));
        });
    }
}
