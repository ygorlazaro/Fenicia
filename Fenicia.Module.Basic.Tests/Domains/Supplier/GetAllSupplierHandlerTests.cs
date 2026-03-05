using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Supplier.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class GetAllSupplierHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllSupplierHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllSupplierHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllSupplierQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Is.Empty);
        Assert.That(result.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_WithSuppliers_ReturnsAllSuppliers()
    {
        // Arrange
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var supplier1 = new BasicSupplierModel
        {
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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

        var query = new GetAllSupplierQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Has.Count.EqualTo(2));
        Assert.That(result.Total, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data[0].PersonId, Is.EqualTo(supplier1.Person.Id));
            Assert.That(result.Data[0].Name, Is.EqualTo(supplier1.Person.Name));
            Assert.That(result.Data[0].Email, Is.EqualTo(supplier1.Person.Email));

            Assert.That(result.Data[1].PersonId, Is.EqualTo(supplier2.Person.Id));
            Assert.That(result.Data[1].Name, Is.EqualTo(supplier2.Person.Name));
            Assert.That(result.Data[1].Email, Is.EqualTo(supplier2.Person.Email));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        for (var i = 0; i < 25; i++)
        {
            var supplier = new BasicSupplierModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new BasicPersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Company.CompanyName()} {i}",
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
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Has.Count.EqualTo(10));
        Assert.That(result.Total, Is.EqualTo(25));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        for (var i = 0; i < 5; i++)
        {
            var supplier = new BasicSupplierModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new BasicPersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Company.CompanyName()} {i}",
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
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Is.Empty);
        Assert.That(result.Total, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var state = new AuthStateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        for (var i = 0; i < 25; i++)
        {
            var supplier = new BasicSupplierModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new BasicPersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Company.CompanyName()} {i}",
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
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Has.Count.EqualTo(10));
        Assert.That(result.Total, Is.EqualTo(25));
    }
}
