using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Supplier.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class GetSupplierByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetSupplierByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetSupplierByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenSupplierExists_ReturnsSupplierResponse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var supplier = new BasicSupplierModel
        {
            Id = supplierId,
            PersonId = Guid.NewGuid(),
            Person = new BasicPersonModel
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
                StateModel = state,
                City = this.faker.Address.City()
            }
        };

        this.context.BasicSuppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(supplierId));
            Assert.That(result.PersonId, Is.EqualTo(supplier.Person.Id));
            Assert.That(result.Name, Is.EqualTo(supplier.Person.Name));
            Assert.That(result.Email, Is.EqualTo(supplier.Person.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(supplier.Person.PhoneNumber));
            Assert.That(result.Document, Is.EqualTo(supplier.Person.Document));
            Assert.That(result.Street, Is.EqualTo(supplier.Person.Street));
            Assert.That(result.Number, Is.EqualTo(supplier.Person.Number));
            Assert.That(result.Complement, Is.EqualTo(supplier.Person.Complement));
            Assert.That(result.Neighborhood, Is.EqualTo(supplier.Person.Neighborhood));
            Assert.That(result.ZipCode, Is.EqualTo(supplier.Person.ZipCode));
            Assert.That(result.StateId, Is.EqualTo(supplier.Person.StateId));
            Assert.That(result.City, Is.EqualTo(supplier.Person.City));
        }
    }

    [Test]
    public async Task Handle_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleSuppliers_ReturnsOnlyRequestedSupplier()
    {
        // Arrange
        var supplier1Id = Guid.NewGuid();
        var supplier2Id = Guid.NewGuid();
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var supplier1 = new BasicSupplierModel
        {
            Id = supplier1Id,
            PersonId = Guid.NewGuid(),
            Person = new BasicPersonModel
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
                StateModel = state,
                City = this.faker.Address.City()
            }
        };

        var supplier2 = new BasicSupplierModel
        {
            Id = supplier2Id,
            PersonId = Guid.NewGuid(),
            Person = new BasicPersonModel
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
                StateModel = state,
                City = this.faker.Address.City()
            }
        };

        this.context.BasicSuppliers.AddRange(supplier1, supplier2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplier1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(supplier1Id));
            Assert.That(result.Name, Is.EqualTo(supplier1.Person.Name));
        }
    }

    [Test]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var supplier = new BasicSupplierModel
        {
            Id = supplierId,
            PersonId = Guid.NewGuid(),
            Person = new BasicPersonModel
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
                StateModel = state,
                City = null,
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicSuppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(supplier.Person.Name));
        Assert.That(result.Email, Is.EqualTo(supplier.Person.Email));
    }
}
