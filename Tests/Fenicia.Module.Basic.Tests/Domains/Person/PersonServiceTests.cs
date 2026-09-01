using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Person;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Person;

public class PersonServiceTests : IDisposable
{
    private readonly Faker _faker;
    private readonly Mock<IPersonRepository> _mockRepository;
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _service = new PersonService(_mockRepository.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task InsertAsync_WhenPersonIsValid_InsertsPerson()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<PersonModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        var result = await _service.InsertAsync(person, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CompanyId.Should().Be(person.CompanyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonExists_UpdatesPerson()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = "Updated Name" };
        _mockRepository.Setup(r => r.UpdateAsync(person.Id, It.IsAny<PersonModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        var result = await _service.UpdateAsync(person.Id, person, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(person.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PersonModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PersonModel?)null);

        // Act
        var result = await _service.UpdateAsync(Guid.NewGuid(), new PersonModel(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonExists_ReturnsPerson()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _mockRepository.Setup(r => r.GetByIdAsync(person.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        var result = await _service.GetByIdAsync(person.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(person.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PersonModel?)null);

        // Act
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
