using Bogus;
using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.UserRole;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class CompanyRepositoryTests
{
    private AuthContext _context;
    private CompanyRepository _sut;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new CompanyRepository(_context);
        _faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CheckCompanyExistsAsync_ById_ReturnsTrue_WhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CheckCompanyExistsAsync(company.Id);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckCompanyExistsAsync_ById_ReturnsFalse_WhenNotExists()
    {
        // Act
        var result = await _sut.CheckCompanyExistsAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckCompanyExistsAsync_ByCnpj_ReturnsTrue_WhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CheckCompanyExistsAsync(company.Cnpj);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Add_SavesCompanyToDatabase()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };

        // Act
        var result = _sut.Add(company);
        await _sut.SaveAsync();

        // Assert
        var savedCompany = await _context.Companies.FindAsync(company.Id);
        Assert.That(savedCompany, Is.Not.Null);
        Assert.That(savedCompany.Name, Is.EqualTo(company.Name));
        Assert.That(savedCompany.Cnpj, Is.EqualTo(company.Cnpj));
    }

    [Test]
    public async Task GetByCnpjAsync_ReturnsCompany_WhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByCnpjAsync(company.Cnpj);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(company.Id));
        Assert.That(result.Cnpj, Is.EqualTo(company.Cnpj));
    }

    [Test]
    public async Task GetByUserIdAsync_ReturnsCompanies_WithPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (int i = 0; i < 15; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(14, "0123456789"),
            };
            companies.Add(company);

            var userRole = new UserRoleModel
            {
                UserId = userId,
                CompanyId = company.Id,
                Company = company,
            };
            userRoles.Add(userRole);
        }

        await _context.Companies.AddRangeAsync(companies);
        await _context.UserRoles.AddRangeAsync(userRoles);
        await _context.SaveChangesAsync();

        // Act
        var page1 = await _sut.GetByUserIdAsync(userId, page: 1, perPage: 10);
        var page2 = await _sut.GetByUserIdAsync(userId, page: 2, perPage: 10);

        // Assert
        Assert.That(page1, Has.Count.EqualTo(10));
        Assert.That(page2, Has.Count.EqualTo(5));
    }

    [Test]
    public async Task CountByUserIdAsync_ReturnsCorrectCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedCount = 5;
        var companies = new List<CompanyModel>();
        var userRoles = new List<UserRoleModel>();

        for (int i = 0; i < expectedCount; i++)
        {
            var company = new CompanyModel
            {
                Id = Guid.NewGuid(),
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(14, "0123456789"),
            };
            companies.Add(company);

            var userRole = new UserRoleModel
            {
                UserId = userId,
                CompanyId = company.Id,
                Company = company,
            };
            userRoles.Add(userRole);
        }

        await _context.Companies.AddRangeAsync(companies);
        await _context.UserRoles.AddRangeAsync(userRoles);
        await _context.SaveChangesAsync();

        // Act
        var count = await _sut.CountByUserIdAsync(userId);

        // Assert
        Assert.That(count, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task PatchAsync_UpdatesCompany()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Random.String2(14, "0123456789"),
        };
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        var updatedName = _faker.Company.CompanyName();
        company.Name = updatedName;

        // Act
        var result = _sut.PatchAsync(company);
        await _sut.SaveAsync();

        // Assert
        var updatedCompany = await _context.Companies.FindAsync(company.Id);
        Assert.That(updatedCompany, Is.Not.Null);
        Assert.That(updatedCompany.Name, Is.EqualTo(updatedName));
    }

    [Test]
    public async Task GetByUserIdAsync_ReturnsEmptyList_WhenUserHasNoCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByUserIdAsync(userId);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
