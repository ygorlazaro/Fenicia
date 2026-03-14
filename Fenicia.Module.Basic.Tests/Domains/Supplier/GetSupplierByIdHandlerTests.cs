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
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetSupplierByIdHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_ReturnsSupplierResponse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        var supplier = new SupplierModel
        {
            Id = supplierId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Phone.PhoneNumber(),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        this.db.BasicSuppliers.Add(supplier);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplierId, result.Id);
        Assert.Equal(supplier.Person.Id, result.PersonId);
        Assert.Equal(supplier.Person.Name, result.Name);
        Assert.Equal(supplier.Person.Email, result.Email);
        Assert.Equal(supplier.Person.PhoneNumber, result.PhoneNumber);
        Assert.Equal(supplier.Person.Document, result.Document);
        Assert.Equal(supplier.Person.Street, result.Street);
        Assert.Equal(supplier.Person.Number, result.Number);
        Assert.Equal(supplier.Person.Complement, result.Complement);
        Assert.Equal(supplier.Person.Neighborhood, result.Neighborhood);
        Assert.Equal(supplier.Person.ZipCode, result.ZipCode);
        Assert.Equal(supplier.Person.StateId, result.StateId);
        Assert.Equal(supplier.Person.City, result.City);
    }

    [Fact]
    public async Task Handle_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleSuppliers_ReturnsOnlyRequestedSupplier()
    {
        // Arrange
        var supplier1Id = Guid.NewGuid();
        var supplier2Id = Guid.NewGuid();
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        var supplier1 = new SupplierModel
        {
            Id = supplier1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Phone.PhoneNumber(),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = supplier2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Phone.PhoneNumber(),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        this.db.BasicSuppliers.AddRange(supplier1, supplier2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplier1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplier1Id, result.Id);
        Assert.Equal(supplier1.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        var supplier = new SupplierModel
        {
            Id = supplierId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = string.Empty,
                Number = string.Empty,
                Complement = null,
                Neighborhood = null,
                ZipCode = string.Empty,
                StateId = state.Id,
                State = state,
                City = null,
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.db.BasicSuppliers.Add(supplier);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(supplier.Person.Name, result.Name);
        Assert.Equal(supplier.Person.Email, result.Email);
    }
}