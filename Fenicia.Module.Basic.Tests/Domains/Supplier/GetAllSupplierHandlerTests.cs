using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Supplier.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class GetAllSupplierHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options);
        this.handler = new GetAllSupplierHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context = null!;
    private GetAllSupplierHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllSupplierQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithSuppliers_ReturnsAllSuppliers()
    {
        // Arrange
        var supplier1 = new BasicSupplier
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new BasicPerson
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email()
            }
        };

        var supplier2 = new BasicSupplier
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new BasicPerson
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email()
            }
        };

        this.context.BasicSuppliers.AddRange(supplier1, supplier2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var supplier = new BasicSupplier
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new BasicPerson
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Company.CompanyName()} {i}"
                }
            };
            this.context.BasicSuppliers.Add(supplier);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
