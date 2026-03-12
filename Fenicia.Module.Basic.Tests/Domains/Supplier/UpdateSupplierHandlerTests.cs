using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class UpdateSupplierHandlerTests : IDisposable
{
    public UpdateSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new UpdateSupplierHandler(this.db);
    }

    private readonly DefaultContext db;
    private readonly UpdateSupplierHandler handler;

    [Fact]
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

        this.db.BasicSuppliers.Add(supplier);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("98.765.432/0001-10",
            result.Cnpj);
    }

    [Fact]
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
            Guid.NewGuid(),
            null,
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
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
            Guid.NewGuid(),
            null,
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
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

        this.db.BasicSuppliers.Add(supplier);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
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
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var updatedSupplier = await this.db.BasicSuppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == supplierId);

        Assert.NotNull(updatedSupplier);
        Assert.Equal("New Name",
            updatedSupplier.Person.Name);
        Assert.Equal("98.765.432/0001-10",
            updatedSupplier.Cnpj);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}
