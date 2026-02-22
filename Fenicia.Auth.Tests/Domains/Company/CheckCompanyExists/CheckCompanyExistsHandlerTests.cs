using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company.CheckCompanyExists;

[TestFixture]
public class CheckCompanyExistsHandlerTests
{
    private AuthContext context = null!;
    private CheckCompanyExistsHandler handler = null!;
    private Faker faker = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new CheckCompanyExistsHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    [Test]
    public async Task Handle_WhenCompanyExistsWithMatchingCnpj_ReturnsTrue()
    {
        // Arrange
        var cnpj = this.faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Cnpj = cnpj,
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new CheckUserExistsQuery(cnpj, false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true when company with matching CNPJ exists");
    }

    [Test]
    public async Task Handle_WhenCompanyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cnpj =  this.faker.Company.Cnpj();
        var query = new CheckUserExistsQuery(cnpj, false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when no company with matching CNPJ exists");
    }

    [Test]
    public async Task Handle_WhenOnlyActiveIsTrueAndCompanyIsActive_ReturnsTrue()
    {
        // Arrange
        var cnpj =  this.faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Cnpj = cnpj,
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new CheckUserExistsQuery(cnpj, true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true when active company exists and OnlyActive is true");
    }

    [Test]
    public async Task Handle_WhenOnlyActiveIsTrueAndCompanyIsInactive_ReturnsFalse()
    {
        // Arrange
        var cnpj =  this.faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Cnpj = cnpj,
            IsActive = false,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new CheckUserExistsQuery(cnpj, true);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when only inactive company exists and OnlyActive is true");
    }

    [Test]
    public async Task Handle_WhenOnlyActiveIsFalseAndCompanyIsInactive_ReturnsTrue()
    {
        // Arrange
        var cnpj =  this.faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Cnpj = cnpj,
            IsActive = false,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new CheckUserExistsQuery(cnpj, false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true when inactive company exists and OnlyActive is false");
    }

    [Test]
    public async Task Handle_WhenMultipleCompaniesExist_OnlyMatchesExactCnpj()
    {
        // Arrange
        var cnpj1 =  this.faker.Company.Cnpj();
        var cnpj2 =  this.faker.Company.Cnpj();

        var company1 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 1",
            Cnpj = cnpj1,
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var company2 = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Company 2",
            Cnpj = cnpj2,
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.AddRange(company1, company2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new CheckUserExistsQuery(cnpj1, false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true for exact CNPJ match");
    }

    [Test]
    public async Task Handle_WhenMixedActiveAndInactiveCompanies_OnlyActiveFilterWorksCorrectly()
    {
        // Arrange
        var cnpj =  this.faker.Company.Cnpj();

        var activeCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Active Company",
            Cnpj = cnpj,
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        var inactiveCompany = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Company",
            Cnpj = cnpj,
            IsActive = false,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.AddRange(activeCompany, inactiveCompany);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var activeQuery = new CheckUserExistsQuery(cnpj, true);
        var inactiveQuery = new CheckUserExistsQuery(cnpj, false);

        // Act
        var activeResult = await this.handler.Handle(activeQuery, CancellationToken.None);
        var inactiveResult = await this.handler.Handle(inactiveQuery, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activeResult, Is.True, "Should find active company when OnlyActive is true");
            Assert.That(inactiveResult, Is.True, "Should find companies regardless of status when OnlyActive is false");
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        // Arrange
        var query = new CheckUserExistsQuery("12345678000195", false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false with empty database");
    }

    [Test]
    public async Task Handle_WhenCnpjContainsSpecialCharacters_NoMatch()
    {
        // Arrange
        var cnpj =  this.faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Cnpj = "12345678000195",
            IsActive = true,
            TimeZone = "UTC",
            Language = "pt-BR"
        };

        this.context.Companies.Add(company);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new CheckUserExistsQuery(cnpj, false);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should not match CNPJ with special characters against plain CNPJ");
    }
}
