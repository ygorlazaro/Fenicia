using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class GetSupplierByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetSupplierByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetSupplierByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenSupplierExists_ReturnsSupplierResponse()
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
                Email = this.faker.Internet.Email()
            }
        };

        this.context.Suppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(supplierId));
            Assert.That(result.Cnpj, Is.EqualTo(supplier.Cnpj));
        }
    }

    [Test]
    public async Task Handle_WhenSupplierDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetSupplierByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}
