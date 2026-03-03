using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Customer.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class GetAllCustomerHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllCustomerHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
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
        var customer1 = new BasicCustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
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

        var customer2 = new BasicCustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
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

        this.context.BasicCustomers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].PersonId, Is.EqualTo(customer1.PersonModel.Id));
            Assert.That(result[1].PersonId, Is.EqualTo(customer2.PersonModel.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var customer = new BasicCustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                PersonModel = new BasicPersonModel
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
            this.context.BasicCustomers.Add(customer);
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
            var customer = new BasicCustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                PersonModel = new BasicPersonModel
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
            this.context.BasicCustomers.Add(customer);
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
            var customer = new BasicCustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                PersonModel = new BasicPersonModel
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
            this.context.BasicCustomers.Add(customer);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
