using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
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
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetCustomerByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetCustomerByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenCustomerExists_ReturnsCustomerResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicCustomers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(customerId));
            Assert.That(result.PersonId, Is.EqualTo(customer.Person.Id));
            Assert.That(result.Name, Is.EqualTo(customer.Person.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Person.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(customer.Person.PhoneNumber));
            Assert.That(result.Document, Is.EqualTo(customer.Person.Document));
            Assert.That(result.Street, Is.EqualTo(customer.Person.Street));
            Assert.That(result.Number, Is.EqualTo(customer.Person.Number));
            Assert.That(result.Complement, Is.EqualTo(customer.Person.Complement));
            Assert.That(result.Neighborhood, Is.EqualTo(customer.Person.Neighborhood));
            Assert.That(result.ZipCode, Is.EqualTo(customer.Person.ZipCode));
            Assert.That(result.StateId, Is.EqualTo(customer.Person.StateId));
            Assert.That(result.City, Is.EqualTo(customer.Person.City));
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
    public async Task Handle_WithMultipleCustomers_ReturnsOnlyRequestedCustomer()
    {
        // Arrange
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
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
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicCustomers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customer1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(customer1Id));
            Assert.That(result.PersonId, Is.EqualTo(customer1.Person.Id));
            Assert.That(result.Name, Is.EqualTo(customer1.Person.Name));
        }
    }

    [Test]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

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
                StateId = state.Id,
                State = state,
                City = null,
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.context.BasicCustomers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo(customer.Person.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Person.Email));
        }
    }
}
