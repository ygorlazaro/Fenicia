using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Supplier.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class DeleteSupplierHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteSupplierHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteSupplierHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenSupplierExists_SetsDeletedDate()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new BasicSupplierModel
        {
            Id = supplierId,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.BasicSuppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplierId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await this.context.BasicSuppliers.FindAsync([supplierId], CancellationToken.None);
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
        var suppliers = await this.context.BasicSuppliers.ToListAsync();
        Assert.That(suppliers, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleSuppliers_OnlyDeletesSpecified()
    {
        // Arrange
        var supplier1Id = Guid.NewGuid();
        var supplier2Id = Guid.NewGuid();

        var supplier1 = new BasicSupplierModel
        {
            Id = supplier1Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName()
            }
        };

        var supplier2 = new BasicSupplierModel
        {
            Id = supplier2Id,
            PersonId = Guid.NewGuid(),
            PersonModel = new BasicPersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName()
            }
        };

        this.context.BasicSuppliers.AddRange(supplier1, supplier2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplier1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await this.context.BasicSuppliers.FindAsync([supplier1Id], CancellationToken.None);
        var notDeletedSupplier = await this.context.BasicSuppliers.FindAsync([supplier2Id], CancellationToken.None);

        Assert.That(deletedSupplier, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedSupplier.Deleted, Is.Not.Null);
            Assert.That(notDeletedSupplier, Is.Not.Null);
        }
        Assert.That(notDeletedSupplier?.Deleted, Is.Null);
    }
}
