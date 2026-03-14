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
        this.db = new DefaultContext(options, companyContext);
        this.handler = new DeleteSupplierHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
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

        this.db.BasicSuppliers.Add(supplier);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplierId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await this.db.BasicSuppliers.FindAsync([supplierId], CancellationToken.None);
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
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var suppliers = await this.db.BasicSuppliers.ToListAsync();
        Assert.Empty(suppliers);
    }

    [Fact]
    public async Task Handle_WithMultipleSuppliers_OnlyDeletesSpecified()
    {
        // Arrange
        var supplier1Id = Guid.NewGuid();
        var supplier2Id = Guid.NewGuid();

        var supplier1 = new SupplierModel { Id = supplier1Id, PersonId = Guid.NewGuid(), Person = new PersonModel { Id = Guid.NewGuid(), Name = this.faker.Company.CompanyName() } };

        var supplier2 = new SupplierModel { Id = supplier2Id, PersonId = Guid.NewGuid(), Person = new PersonModel { Id = Guid.NewGuid(), Name = this.faker.Company.CompanyName() } };

        this.db.BasicSuppliers.AddRange(supplier1, supplier2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSupplierCommand(supplier1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSupplier = await this.db.BasicSuppliers.FindAsync([supplier1Id], CancellationToken.None);
        var notDeletedSupplier = await this.db.BasicSuppliers.FindAsync([supplier2Id], CancellationToken.None);

        Assert.NotNull(deletedSupplier);
        Assert.NotNull(deletedSupplier.Deleted);
        Assert.NotNull(notDeletedSupplier);
        Assert.Null(notDeletedSupplier.Deleted);
    }
}