namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;
using Fenicia.Auth.Domains.Company;

public class CompanyRepositoryTests
{
    private CancellationToken _cancellationToken;
    private AuthContext _context;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;
    private CompanyRepository _sut;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        var mockLogger = new Mock<ILogger<CompanyRepository>>().Object;

        _context = new AuthContext(_options);
        _sut = new CompanyRepository(_context, mockLogger);
        _faker = new Faker();
        _cancellationToken = CancellationToken.None;
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
            Cnpj = _faker.Random.String2(length: 14, "0123456789")
        };
        await _context.Companies.AddAsync(company, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.CheckCompanyExistsAsync(company.Id, _cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckCompanyExistsAsync_ById_ReturnsFalse_WhenNotExists()
    {
        // Act
        var result = await _sut.CheckCompanyExistsAsync(Guid.NewGuid(), _cancellationToken);

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
            Cnpj = _faker.Random.String2(length: 14, "0123456789")
        };
        await _context.Companies.AddAsync(company, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.CheckCompanyExistsAsync(company.Cnpj, _cancellationToken);

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
            Cnpj = _faker.Random.String2(length: 14, "0123456789")
        };

        // Act
        _sut.Add(company);
        await _sut.SaveAsync(_cancellationToken);

        // Assert
        var savedCompany = await _context.Companies.FindAsync([company.Id], _cancellationToken);
        Assert.That(savedCompany, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(savedCompany.Name, Is.EqualTo(company.Name));
            Assert.That(savedCompany.Cnpj, Is.EqualTo(company.Cnpj));
        });
    }

    [Test]
    public async Task GetByCnpjAsync_ReturnsCompany_WhenExists()
    {
        // Arrange
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Random.String2(length: 14, "0123456789")
        };
        await _context.Companies.AddAsync(company, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetByCnpjAsync(company.Cnpj, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(company.Id));
            Assert.That(result.Cnpj, Is.EqualTo(company.Cnpj));
        });
    }

    [Test]
    public async Task GetByUserIdAsync_ReturnsCompanies_WithPagination()
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
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(length: 14, "0123456789")
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

        await _context.Companies.AddRangeAsync(companies, _cancellationToken);
        await _context.UserRoles.AddRangeAsync(userRoles, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var page1 = await _sut.GetByUserIdAsync(userId, _cancellationToken, page: 1, perPage: 10);
        var page2 = await _sut.GetByUserIdAsync(userId, _cancellationToken, page: 2, perPage: 10);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(page1, Has.Count.EqualTo(expected: 10));
            Assert.That(page2, Has.Count.EqualTo(expected: 5));
        });
    }

    [Test]
    public async Task CountByUserIdAsync_ReturnsCorrectCount()
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
                Name = _faker.Company.CompanyName(),
                Cnpj = _faker.Random.String2(length: 14, "0123456789")
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

        await _context.Companies.AddRangeAsync(companies, _cancellationToken);
        await _context.UserRoles.AddRangeAsync(userRoles, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var count = await _sut.CountByUserIdAsync(userId, _cancellationToken);

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
            Cnpj = _faker.Random.String2(length: 14, "0123456789")
        };
        await _context.Companies.AddAsync(company, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        var updatedName = _faker.Company.CompanyName();
        company.Name = updatedName;

        // Act
        _sut.PatchAsync(company);
        await _sut.SaveAsync(_cancellationToken);

        // Assert
        var updatedCompany = await _context.Companies.FindAsync([company.Id], _cancellationToken);
        Assert.That(updatedCompany, Is.Not.Null);
        Assert.That(updatedCompany.Name, Is.EqualTo(updatedName));
    }

    [Test]
    public async Task GetByUserIdAsync_ReturnsEmptyList_WhenUserHasNoCompanies()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByUserIdAsync(userId, _cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
