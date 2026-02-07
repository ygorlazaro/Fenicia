using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        this.context = new AuthContext(this.options);
        this.sut = new CompanyRepository(this.context);
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
    public async Task CheckCompanyExistsAsyncByIdReturnsTrueWhenExists()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.CheckCompanyExistsAsync(company.Id, true, this.cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByIdReturnsFalseWhenNotExists()
    {
        var result = await this.sut.CheckCompanyExistsAsync(Guid.NewGuid(), true, this.cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckCompanyExistsAsyncByCnpjReturnsTrueWhenExists()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.CheckCompanyExistsAsync(company.Cnpj, true, this.cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AddSavesCompanyToDatabase()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789")
        };

        this.sut.Add(company);
        await this.sut.SaveChangesAsync(this.cancellationToken);

        var savedCompany = await this.context.Companies.FindAsync([company.Id], this.cancellationToken);
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
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetByCnpjAsync(company.Cnpj, true, this.cancellationToken);

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
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789")
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

        var page1 = await this.sut.GetByUserIdAsync(userId, true, this.cancellationToken);
        var page2 = await this.sut.GetByUserIdAsync(userId, true, this.cancellationToken, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page1, Has.Count.EqualTo(10));
            Assert.That(page2, Has.Count.EqualTo(5));
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
                Name = this.faker.Company.CompanyName(),
                Cnpj = this.faker.Random.String2(14, "0123456789")
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

        var count = await this.sut.CountByUserIdAsync(userId, true, this.cancellationToken);

        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task PatchAsyncUpdatesCompany()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789")
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var updatedName = this.faker.Company.CompanyName();
        company.Name = updatedName;

        this.sut.Update(company);
        await this.sut.SaveChangesAsync(this.cancellationToken);

        var updatedCompany = await this.context.Companies.FindAsync([company.Id], this.cancellationToken);
        Assert.That(updatedCompany, Is.Not.Null);
        Assert.That(updatedCompany.Name, Is.EqualTo(updatedName));
    }

    [Test]
    public async Task GetByUserIdAsyncReturnsEmptyListWhenUserHasNoCompanies()
    {
        var userId = Guid.NewGuid();

        var result = await this.sut.GetByUserIdAsync(userId, true, this.cancellationToken);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CheckCompanyExistsRespectsOnlyActiveFlagByIdAndCnpj()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789"),
            IsActive = false
        };
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var existsOnlyActiveById = await this.sut.CheckCompanyExistsAsync(company.Id, true, this.cancellationToken);
        var existsAnyById = await this.sut.CheckCompanyExistsAsync(company.Id, false, this.cancellationToken);

        var existsOnlyActiveByCnpj = await this.sut.CheckCompanyExistsAsync(company.Cnpj, true, this.cancellationToken);
        var existsAnyByCnpj = await this.sut.CheckCompanyExistsAsync(company.Cnpj, false, this.cancellationToken);

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
            Name = this.faker.Company.CompanyName(),
            Cnpj = this.faker.Random.String2(14, "0123456789"),
            IsActive = true
        };
        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName(),
            Cnpj = activeCompany.Cnpj,
            IsActive = false
        };

        await this.context.Companies.AddAsync(inactiveCompany, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var resultOnlyActive = await this.sut.GetByCnpjAsync(activeCompany.Cnpj, true, this.cancellationToken);
        var resultAny = await this.sut.GetByCnpjAsync(activeCompany.Cnpj, false, this.cancellationToken);

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
        var activeCompany = new CompanyModel { Id = Guid.NewGuid(), Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Random.String2(14, "0123456789"), IsActive = true };
        var inactiveCompany = new CompanyModel { Id = Guid.NewGuid(), Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Random.String2(14, "0123456789"), IsActive = false };

        var userRoles = new List<UserRoleModel>
        {
            new() { UserId = userId, CompanyId = activeCompany.Id, Company = activeCompany },
            new() { UserId = userId, CompanyId = inactiveCompany.Id, Company = inactiveCompany }
        };

        await this.context.Companies.AddRangeAsync([activeCompany, inactiveCompany], this.cancellationToken);
        await this.context.UserRoles.AddRangeAsync(userRoles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var onlyActive = await this.sut.GetByUserIdAsync(userId, true, this.cancellationToken);
        var any = await this.sut.GetByUserIdAsync(userId, false, this.cancellationToken);

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
        var activeCompany = new CompanyModel { Id = Guid.NewGuid(), Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Random.String2(14, "0123456789"), IsActive = true };
        var inactiveCompany = new CompanyModel { Id = Guid.NewGuid(), Name = this.faker.Company.CompanyName(), Cnpj = this.faker.Random.String2(14, "0123456789"), IsActive = false };

        var userRoles = new List<UserRoleModel>
        {
            new() { UserId = userId, CompanyId = activeCompany.Id },
            new() { UserId = userId, CompanyId = activeCompany.Id },
            new() { UserId = userId, CompanyId = inactiveCompany.Id }
        };

        await this.context.Companies.AddRangeAsync([activeCompany, inactiveCompany], this.cancellationToken);
        await this.context.UserRoles.AddRangeAsync(userRoles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var onlyActive = await this.sut.GetCompaniesAsync(userId, true, this.cancellationToken);
        var any = await this.sut.GetCompaniesAsync(userId, false, this.cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(onlyActive, Has.Count.EqualTo(1));
            Assert.That(onlyActive, Does.Contain(activeCompany.Id));
            Assert.That(any, Has.Count.EqualTo(2));
            Assert.That(any, Does.Contain(inactiveCompany.Id));
        });
    }
}