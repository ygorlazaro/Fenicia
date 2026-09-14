using AwesomeAssertions;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.PersonAddress;

public class PersonAddressServiceTests : IDisposable
{
    private readonly Mock<IPersonAddressRepository> _mockRepository;
    private readonly PersonAddressService _service;

    public PersonAddressServiceTests()
    {
        _mockRepository = new Mock<IPersonAddressRepository>();
        _service = new PersonAddressService(_mockRepository.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task InsertAsync_WhenPersonAddressIsValid_InsertsPersonAddress()
    {
        // Arrange
        var personAddress = new PersonAddressModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            AddressId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<PersonAddressModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(personAddress);

        // Act
        var result = await _service.InsertAsync(personAddress, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(personAddress.Id);
    }
}