using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Person;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Person;

public class PersonServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new PersonRepository(_db);
        _service = new PersonService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonExists_ReturnsPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(person.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenPersonIsValid_InsertsPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };

        var result = await _service.InsertAsync(person, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonExists_UpdatesPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        person.Name = "Updated Name";
        var result = await _service.UpdateAsync(person.Id, person, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };

        var result = await _service.UpdateAsync(person.Id, person, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }
}
