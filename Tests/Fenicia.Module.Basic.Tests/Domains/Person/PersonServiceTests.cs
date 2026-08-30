using AwesomeAssertions;
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
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(person.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(person.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenPersonIsValid_InsertsPerson()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };

        // Act
        var result = await _service.InsertAsync(person, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonExists_UpdatesPerson()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        await _db.SaveChangesAsync(CancellationToken.None);

        person.Name = "Updated Name";

        // Act
        var result = await _service.UpdateAsync(person.Id, person, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };

        // Act
        var result = await _service.UpdateAsync(person.Id, person, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
