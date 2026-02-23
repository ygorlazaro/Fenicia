using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class UpdateSupplierHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new UpdateSupplierHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private UpdateSupplierHandler handler = null!;

    [Test]
    public async Task Handle_WhenSupplierExists_UpdatesSupplierAndReturnsResponse()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new SupplierModel
        {
            Id = supplierId,
            Cnpj = "12.345.678/0001-90",
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com"
            }
        };

        this.context.Suppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            "Suite 200",
            "New Neighborhood",
            "200",
            Guid.NewGuid(),
            "New Street",
            "54321-000",
            "(11) 98765-4321",
            "98.765.432/0001-10");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Person.Name, Is.EqualTo("New Name"));
            Assert.That(result.Cnpj, Is.EqualTo("98.765.432/0001-10"));
        }
    }

    [Test]
    public async Task Handle_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateSupplierCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            null,
            null,
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateSupplierCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            null,
            null,
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesSupplierWasUpdatedInDatabase()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new SupplierModel
        {
            Id = supplierId,
            Cnpj = "12.345.678/0001-90",
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com"
            }
        };

        this.context.Suppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
            null,
            null,
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            "98.765.432/0001-10");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedSupplier = await this.context.Suppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == supplierId);

        Assert.That(updatedSupplier, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedSupplier.Person.Name, Is.EqualTo("New Name"));
            Assert.That(updatedSupplier.Cnpj, Is.EqualTo("98.765.432/0001-10"));
        }
    }
}
