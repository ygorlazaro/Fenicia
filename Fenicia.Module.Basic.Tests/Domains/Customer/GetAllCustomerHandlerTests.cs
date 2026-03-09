using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Total, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task Handle_WithCustomers_ReturnsAllCustomers()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
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
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicCustomers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(2));
            Assert.That(result.Total, Is.EqualTo(2));
        }
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data[0].PersonId, Is.EqualTo(customer1.Person.Id));
            Assert.That(result.Data[0].Name, Is.EqualTo(customer1.Person.Name));
            Assert.That(result.Data[0].Email, Is.EqualTo(customer1.Person.Email));

            Assert.That(result.Data[1].PersonId, Is.EqualTo(customer2.Person.Id));
            Assert.That(result.Data[1].Name, Is.EqualTo(customer2.Person.Name));
            Assert.That(result.Data[1].Email, Is.EqualTo(customer2.Person.Email));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                    StateId = state.Id,
                    State = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(10));
            Assert.That(result.Total, Is.EqualTo(25));
        }
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                    StateId = state.Id,
                    State = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Total, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                    StateId = state.Id,
                    State = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(10));
            Assert.That(result.Total, Is.EqualTo(25));
        }
    }
}
