using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class DeleteSupplierHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteSupplierHandler handler;

    public DeleteSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteSupplierHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
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
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = faker.Address.City()
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplierId);
        var beforeDelete = DateTime.Now;

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await db.BasicSuppliers.FindAsync([supplierId], CancellationToken.None);
        Assert.NotNull(deletedSupplier);
        Assert.NotNull(deletedSupplier.Deleted);
        Assert.True(deletedSupplier.Deleted >= beforeDelete.AddSeconds(-1));
        Assert.True(deletedSupplier.Deleted <= DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenSupplierDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteSupplierCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var suppliers = await db.BasicSuppliers.ToListAsync();
        Assert.Empty(suppliers);
    }

    [Fact]
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
                Name = faker.Company.CompanyName()
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = supplier2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName()
            }
        };

        db.BasicSuppliers.AddRange(supplier1, supplier2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplier1Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await db.BasicSuppliers.FindAsync([supplier1Id], CancellationToken.None);
        var notDeletedSupplier = await db.BasicSuppliers.FindAsync([supplier2Id], CancellationToken.None);

        Assert.NotNull(deletedSupplier);
        Assert.NotNull(deletedSupplier.Deleted);
        Assert.NotNull(notDeletedSupplier);
        Assert.Null(notDeletedSupplier.Deleted);
    }
}
