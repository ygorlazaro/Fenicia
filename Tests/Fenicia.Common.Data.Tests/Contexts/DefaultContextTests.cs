using AwesomeAssertions;
using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Tests.Contexts;

public class DefaultContextTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;

    public DefaultContextTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetCreated_OnAddedEntity()
    {
        var company = new CompanyModel { Name = _faker.Company.CompanyName(), Cnpj = _faker.Company.Cnpj(), IsActive = true };

        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        company.Created.Should().BeOnOrAfter(DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetUpdated_OnModifiedEntity()
    {
        var company = new CompanyModel { Name = _faker.Company.CompanyName(), Cnpj = _faker.Company.Cnpj(), IsActive = true };
        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        company.Name = "Updated";
        await _db.SaveChangesAsync(CancellationToken.None);

        company.Updated.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetDeleted_OnDeletedEntity()
    {
        var company = new CompanyModel { Name = _faker.Company.CompanyName(), Cnpj = _faker.Company.Cnpj(), IsActive = true };
        _db.AuthCompanies.Add(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        _db.AuthCompanies.Remove(company);
        await _db.SaveChangesAsync(CancellationToken.None);

        company.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldExcludeSoftDeleted_ForBaseModel()
    {
        var user = new UserModel { Email = _faker.Internet.Email(), Name = _faker.Person.FullName, Password = _faker.Internet.Password() };
        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        _db.AuthUsers.Remove(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _db.AuthUsers.FirstOrDefaultAsync(u => u.Id == user.Id, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task SoftDeletedEntity_ShouldBeIncluded_WhenIgnoringQueryFilters()
    {
        var user = new UserModel { Email = _faker.Internet.Email(), Name = _faker.Person.FullName, Password = _faker.Internet.Password() };
        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        _db.AuthUsers.Remove(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _db.AuthUsers.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == user.Id, CancellationToken.None);
        result.Should().NotBeNull();
        result!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldExcludeSoftDeleted_ForBaseModel()
    {
        _db.AuthUsers.Add(new UserModel { Email = _faker.Internet.Email(), Name = _faker.Person.FullName, Password = _faker.Internet.Password() });
        _db.AuthUsers.Add(new UserModel { Email = _faker.Internet.Email(), Name = _faker.Person.FullName, Password = _faker.Internet.Password() });
        await _db.SaveChangesAsync(CancellationToken.None);

        var first = await _db.AuthUsers.FirstAsync(u => true, CancellationToken.None);
        _db.AuthUsers.Remove(first);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _db.AuthUsers.ToListAsync(CancellationToken.None);
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task BaseCompanyModel_ShouldBeFilteredByCompanyId_WhenCurrentCompanyIdIsSet()
    {
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new DefaultContext(options, companyContext);

        var company1 = new CompanyModel { Id = Guid.NewGuid(), Name = _faker.Company.CompanyName(), Cnpj = _faker.Company.Cnpj(), IsActive = true };
        var company2 = new CompanyModel { Id = Guid.NewGuid(), Name = _faker.Company.CompanyName(), Cnpj = _faker.Company.Cnpj(), IsActive = true };
        db.AuthCompanies.Add(company1);
        db.AuthCompanies.Add(company2);
        await db.SaveChangesAsync(CancellationToken.None);

        var config1 = new ConfigurationModel { CompanyId = company1.Id, ConfigType = ConfigType.Language, Value = "Value1", UserId = Guid.NewGuid() };
        var config2 = new ConfigurationModel { CompanyId = company2.Id, ConfigType = ConfigType.Language, Value = "Value2", UserId = Guid.NewGuid() };
        db.AuthConfigurations.Add(config1);
        db.AuthConfigurations.Add(config2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await db.AuthConfigurations.ToListAsync(CancellationToken.None);
        result.Should().BeEmpty();
    }
}
