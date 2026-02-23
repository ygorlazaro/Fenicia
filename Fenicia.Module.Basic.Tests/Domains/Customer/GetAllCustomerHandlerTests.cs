using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class GetAllCustomerHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetAllCustomerHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetAllCustomerHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithCustomers_ReturnsAllCustomers()
    {
        // Arrange
        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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

        this.context.Customers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Person.Name, Is.EqualTo(customer1.Person.Name));
            Assert.That(result[1].Person.Name, Is.EqualTo(customer2.Person.Name));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
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
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
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
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
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
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_VerifiesPersonDataIsIncluded()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
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

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Person.Name, Is.Not.Null);
            Assert.That(result[0].Person.Email, Is.Not.Null);
            Assert.That(result[0].Person.Address, Is.Not.Null);
        }
    }
}
