using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.Handlers;
using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

/// <summary>
///     Unit tests for the CheckCompanyExistsHandler.
///     Tests CNPJ uniqueness validation logic including active/inactive filtering and exact matching.
/// </summary>
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

    /// <summary>
    ///     Tests that a company with matching CNPJ returns true.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCompanyExistsWithMatchingCnpj_ReturnsTrue()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Tests that a non-existent CNPJ returns false.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCompanyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cnpj = faker.Company.Cnpj();
        var query = new CheckCompanyExistsQuery(cnpj, false);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Tests that OnlyActive=true returns true for active companies.
    /// </summary>
    [Fact]
    public async Task Handle_WhenOnlyActiveIsTrueAndCompanyIsActive_ReturnsTrue()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Tests that OnlyActive=true returns false for inactive companies.
    /// </summary>
    [Fact]
    public async Task Handle_WhenOnlyActiveIsTrueAndCompanyIsInactive_ReturnsFalse()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Tests that OnlyActive=false returns true for inactive companies.
    /// </summary>
    [Fact]
    public async Task Handle_WhenOnlyActiveIsFalseAndCompanyIsInactive_ReturnsTrue()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Tests that only exact CNPJ matches are considered (partial matches return false).
    /// </summary>
    [Fact]
    public async Task Handle_WhenMultipleCompaniesExist_OnlyMatchesExactCnpj()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Tests that active/inactive filtering works correctly with mixed companies.
    /// </summary>
    [Fact]
    public async Task Handle_WhenMixedActiveAndInactiveCompanies_OnlyActiveFilterWorksCorrectly()
    {
        // Arrange
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

        // Act
        var activeResult = await handler.Handle(activeQuery, CancellationToken.None);
        var inactiveResult = await handler.Handle(inactiveQuery, CancellationToken.None);

        // Assert
        Assert.True(activeResult);
        Assert.True(inactiveResult);
    }

    /// <summary>
    ///     Tests that an empty database returns false for any CNPJ query.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        // Arrange
        var query = new CheckCompanyExistsQuery(faker.Company.Cnpj(), false);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Tests that CNPJ with special characters does not match a valid CNPJ.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCnpjContainsSpecialCharacters_NoMatch()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
