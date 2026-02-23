using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class DeleteSupplierHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new DeleteSupplierHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private DeleteSupplierHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenSupplierExists_SetsDeletedDate()
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
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Cpf = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Suppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplierId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await this.context.Suppliers.FindAsync([supplierId], CancellationToken.None);
        Assert.That(deletedSupplier, Is.Not.Null);
        Assert.That(deletedSupplier.Deleted, Is.Not.Null);
        Assert.That(deletedSupplier.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedSupplier.Deleted, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenSupplierDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteSupplierCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var suppliers = await this.context.Suppliers.ToListAsync();
        Assert.That(suppliers, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleSuppliers_OnlyDeletesSpecified()
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
                Name = this.faker.Company.CompanyName()
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = supplier2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName()
            }
        };

        this.context.Suppliers.AddRange(supplier1, supplier2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplier1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await this.context.Suppliers.FindAsync([supplier1Id], CancellationToken.None);
        var notDeletedSupplier = await this.context.Suppliers.FindAsync([supplier2Id], CancellationToken.None);

        Assert.That(deletedSupplier, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedSupplier.Deleted, Is.Not.Null);
            Assert.That(notDeletedSupplier, Is.Not.Null);
        }
        Assert.That(notDeletedSupplier?.Deleted, Is.Null);
    }
}
