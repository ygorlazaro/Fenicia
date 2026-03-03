using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Customer.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class DeleteCustomerHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteCustomerHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteCustomerHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenCustomerExists_SetsDeletedDate()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new BasicCustomerModel
        {
            Id = customerId,
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

        this.context.BasicCustomers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteCustomerCommand(customerId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCustomer = await this.context.BasicCustomers.FindAsync([customerId], CancellationToken.None);
        Assert.That(deletedCustomer, Is.Not.Null);
        Assert.That(deletedCustomer.Deleted, Is.Not.Null);
        Assert.That(deletedCustomer.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedCustomer.Deleted, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenCustomerDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var customers = await this.context.BasicCustomers.ToListAsync();
        Assert.That(customers, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleCustomers_OnlyDeletesSpecified()
    {
        // Arrange
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var customer1 = new BasicCustomerModel
        {
            Id = customer1Id,
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
            Id = customer2Id,
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

        var command = new DeleteCustomerCommand(customer1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCustomer = await this.context.BasicCustomers.FindAsync([customer1Id], CancellationToken.None);
        var notDeletedCustomer = await this.context.BasicCustomers.FindAsync([customer2Id], CancellationToken.None);

        Assert.That(deletedCustomer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedCustomer.Deleted, Is.Not.Null);
            Assert.That(notDeletedCustomer, Is.Not.Null);
        }
        Assert.That(notDeletedCustomer?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var customers = await this.context.BasicCustomers.ToListAsync();
        Assert.That(customers, Is.Empty);
    }
}
