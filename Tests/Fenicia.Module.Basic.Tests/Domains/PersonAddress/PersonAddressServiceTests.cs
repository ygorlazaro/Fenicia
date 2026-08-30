using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.PersonAddress;

public class PersonAddressServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PersonAddressService _service;

    public PersonAddressServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new PersonAddressRepository(_db);
        _service = new PersonAddressService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task InsertAsync_WhenValid_SetsCompanyIdAndInserts()
    {
        // Arrange
        var personAddress = new PersonAddressModel
        {
            PersonId = Guid.NewGuid(),
            AddressId = Guid.NewGuid()
        };

        // Act
        var result = await _service.InsertAsync(personAddress, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CompanyId.Should().Be(_db.CurrentCompanyId ?? Guid.Empty);
        result.Id.Should().NotBeEmpty();
    }
}
