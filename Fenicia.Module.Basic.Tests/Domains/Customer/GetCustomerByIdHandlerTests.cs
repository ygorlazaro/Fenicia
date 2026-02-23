using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class GetCustomerByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetCustomerByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetCustomerByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenCustomerExists_ReturnsCustomerResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(customerId));
            Assert.That(result.Person.Name, Is.EqualTo(customer.Person.Name));
            Assert.That(result.Person.Email, Is.EqualTo(customer.Person.Email));
        }
    }

    [Test]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesPersonDataIsIncluded()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Person.Name, Is.Not.Null);
            Assert.That(result.Person.Email, Is.Not.Null);
            Assert.That(result.Person.Address, Is.Not.Null);
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Person.Address?.City, Is.EqualTo(customer.Person.City));
            Assert.That(result.Person.Address.Street, Is.EqualTo(customer.Person.Street));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCustomers_ReturnsOnlyRequestedCustomer()
    {
        // Arrange
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var customer1 = new CustomerModel
        {
            Id = customer1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        var customer2 = new CustomerModel
        {
            Id = customer2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FirstName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customer1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(customer1Id));
            Assert.That(result.Person.Name, Is.EqualTo(customer1.Person.Name));
        }
        Assert.That(result.Person.Name, Is.Not.EqualTo(customer2.Person.Name));
    }

    [Test]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = string.Empty,
                Number = string.Empty,
                Complement = null,
                Neighborhood = null,
                ZipCode = string.Empty,
                StateId = Guid.NewGuid(),
                City = null
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Person.Address?.Complement, Is.Null);
            Assert.That(result.Person.Address?.Neighborhood, Is.Null);
            Assert.That(result.Person.Address?.City, Is.Null);
        }
    }
}
