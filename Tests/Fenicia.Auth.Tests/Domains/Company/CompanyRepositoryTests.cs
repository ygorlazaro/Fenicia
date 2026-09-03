using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Company;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Company;

public class CompanyRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CompanyRepository _repository;

    public CompanyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new CompanyRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenCompanyExists_ReturnsCompany()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByCnpjAsync(cnpj, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
        Assert.Equal(cnpj, result.Cnpj);
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByCnpjAsync(_faker.Company.Cnpj(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenCompanyIsDeleted_ReturnsNull()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true,
            Deleted = DateTime.UtcNow
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByCnpjAsync(cnpj, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyActiveAsync_WhenCompanyIsActive_ReturnsCompany()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyActiveAsync(company.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(company.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task AnyActiveAsync_WhenCompanyIsInactive_ReturnsNull()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = false
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyActiveAsync(company.Id, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyActiveAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        var result = await _repository.AnyActiveAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyAsync_WhenCompanyExists_ReturnsTrue()
    {
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = _faker.Company.Cnpj(),
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(company.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_WhenCompanyDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.AnyAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WithMatchingCnpjAndActiveCompany_ReturnsTrue()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, true, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WithMatchingCnpjAndOnlyActiveFalse_ReturnsTrue()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = true
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WithNoMatch_ReturnsFalse()
    {
        var result = await _repository.CheckExistsAsync(_faker.Company.Cnpj(), false, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsTrueAndCompanyIsInactive_ReturnsFalse()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, true, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckExistsAsync_WhenOnlyActiveIsFalseAndCompanyIsInactive_ReturnsTrue()
    {
        var cnpj = _faker.Company.Cnpj();
        var company = new CompanyModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Company.CompanyName(),
            Cnpj = cnpj,
            IsActive = false
        };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CheckExistsAsync(cnpj, false, CancellationToken.None);

        Assert.True(result);
    }
}