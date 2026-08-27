using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.Handlers;
using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

public class CheckCompanyExistsHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CheckCompanyExistsHandler handler;

    public CheckCompanyExistsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid()
                .ToString())
            .Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new CheckCompanyExistsHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenCompanyExistsWithMatchingCnpj_ReturnsTrue()
    {

        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new CheckCompanyExistsQuery(cnpj, false);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ReturnsFalse()
    {

        var cnpj = faker.Company.Cnpj();
        var query = new CheckCompanyExistsQuery(cnpj, false);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenOnlyActiveIsTrueAndCompanyIsActive_ReturnsTrue()
    {

        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new CheckCompanyExistsQuery(cnpj, true);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WhenOnlyActiveIsTrueAndCompanyIsInactive_ReturnsFalse()
    {

        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new CheckCompanyExistsQuery(cnpj, true);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenOnlyActiveIsFalseAndCompanyIsInactive_ReturnsTrue()
    {

        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new CheckCompanyExistsQuery(cnpj, false);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleCompaniesExist_OnlyMatchesExactCnpj()
    {

        var cnpj1 = faker.Company.Cnpj();
        var cnpj2 = faker.Company.Cnpj();

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj1,
            IsActive = true
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj2,
            IsActive = true
        };

        db.AuthCompanies.AddRange(company1, company2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new CheckCompanyExistsQuery(cnpj1, false);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WhenMixedActiveAndInactiveCompanies_OnlyActiveFilterWorksCorrectly()
    {

        var cnpj = faker.Company.Cnpj();

        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        db.AuthCompanies.AddRange(activeCompany, inactiveCompany);
        await db.SaveChangesAsync(CancellationToken.None);

        var activeQuery = new CheckCompanyExistsQuery(cnpj, true);
        var inactiveQuery = new CheckCompanyExistsQuery(cnpj, false);

        var activeResult = await handler.Handle(activeQuery, CancellationToken.None);
        var inactiveResult = await handler.Handle(inactiveQuery, CancellationToken.None);

        Assert.True(activeResult);
        Assert.True(inactiveResult);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {

        var query = new CheckCompanyExistsQuery(faker.Company.Cnpj(), false);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenCnpjContainsSpecialCharacters_NoMatch()
    {

        var cnpj = faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Cnpj = string.Concat(faker.Company.Cnpj(),
                "./"),
            IsActive = true
        };

        db.AuthCompanies.Add(company);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new CheckCompanyExistsQuery(cnpj, false);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result);
    }
}
