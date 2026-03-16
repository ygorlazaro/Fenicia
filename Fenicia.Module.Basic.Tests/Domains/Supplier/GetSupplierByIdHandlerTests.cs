using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;
using Fenicia.Module.Basic.Domains.Supplier.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class GetSupplierByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetSupplierByIdHandler handler;

    public GetSupplierByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetSupplierByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_ReturnsSupplierResponse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();

        var supplier = new SupplierModel
        {
            Id = supplierId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplierId, result.Id);
        Assert.Equal(supplier.Person.Id, result.PersonId);
        Assert.Equal(supplier.Person.Name, result.Name);
        Assert.Equal(supplier.Person.Email, result.Email);
        Assert.Equal(supplier.Person.PhoneNumber, result.PhoneNumber);
        Assert.Equal(supplier.Person.Document, result.Document);
    }

    [Fact]
    public async Task Handle_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleSuppliers_ReturnsOnlyRequestedSupplier()
    {
        // Arrange
        var supplier1Id = Guid.NewGuid();
        var supplier2Id = Guid.NewGuid();

        var supplier1 = new SupplierModel
        {
            Id = supplier1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = supplier2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        db.BasicSuppliers.AddRange(supplier1, supplier2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplier1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplier1Id, result.Id);
        Assert.Equal(supplier1.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithAddress_ReturnsAddress()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = stateId,
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var addressId = Guid.NewGuid();
        var address = new AddressModel
        {
            Id = addressId,
            Street = faker.Address.StreetName(),
            Number = faker.Random.Replace("####"),
            ZipCode = faker.Address.ZipCode(),
            StateId = stateId,
            City = faker.Address.City(),
            Country = "Brasil"
        };
        db.AuthAddresses.Add(address);

        var personId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();

        var supplier = new SupplierModel
        {
            Id = supplierId,
            PersonId = personId,
            Person = new PersonModel
            {
                Id = personId,
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var personAddress = new PersonAddressModel
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            AddressId = addressId
        };
        db.BasicPersonAddresses.Add(personAddress);

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Address);
        Assert.Equal(addressId, result.Address.Id);
    }
}
