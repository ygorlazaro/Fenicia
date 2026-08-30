using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Person;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Person;

public class PersonRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PersonRepository _repository;

    public PersonRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new PersonRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPeople()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonExists_ReturnsPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(person.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenPersonIsValid_InsertsPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };

        var result = await _repository.InsertAsync(person, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonExists_UpdatesPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(person.Id, person, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };

        var result = await _repository.UpdateAsync(person.Id, person, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenPersonExists_SoftDeletesPerson()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(person.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedPerson = await _db.BasicPeople.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == person.Id);
        Assert.NotNull(deletedPerson);
        Assert.NotNull(deletedPerson.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingPeople()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(p => p.Id == person.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenPersonExists_ReturnsTrue()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(p => p.Id == person.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName() };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(p => p.Id == person.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
