using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;
using Fenicia.Module.Basic.Domains.Supplier.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class GetAllSupplierHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllSupplierHandler handler;

    public GetAllSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllSupplierHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllSupplierQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_WithSuppliers_ReturnsAllSuppliers()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var person1Id = Guid.NewGuid();
        var person2Id = Guid.NewGuid();

        var supplier1 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PersonId = person1Id,
            Person = new PersonModel
            {
                Id = person1Id,
                CompanyId = companyId,
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PersonId = person2Id,
            Person = new PersonModel
            {
                Id = person2Id,
                CompanyId = companyId,
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicSuppliers.AddRange(supplier1, supplier2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Equal(supplier1.Person.Id, result.Data[0].PersonId);
        Assert.Equal(supplier1.Person.Name, result.Data[0].Name);
        Assert.Equal(supplier1.Person.Email, result.Data[0].Email);

        Assert.Equal(supplier2.Person.Id, result.Data[1].PersonId);
        Assert.Equal(supplier2.Person.Name, result.Data[1].Name);
        Assert.Equal(supplier2.Person.Email, result.Data[1].Email);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        for (var i = 0; i < 25; i++)
        {
            var personId = Guid.NewGuid();
            var supplier = new SupplierModel
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PersonId = personId,
                Person = new PersonModel
                {
                    Id = personId,
                    CompanyId = companyId,
                    Name = $"{faker.Company.CompanyName()} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicSuppliers.Add(supplier);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery(2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        for (var i = 0; i < 5; i++)
        {
            var personId = Guid.NewGuid();
            var supplier = new SupplierModel
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PersonId = personId,
                Person = new PersonModel
                {
                    Id = personId,
                    CompanyId = companyId,
                    Name = $"{faker.Company.CompanyName()} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicSuppliers.Add(supplier);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery(10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(5, result.Total);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        for (var i = 0; i < 25; i++)
        {
            var personId = Guid.NewGuid();
            var supplier = new SupplierModel
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PersonId = personId,
                Person = new PersonModel
                {
                    Id = personId,
                    CompanyId = companyId,
                    Name = $"{faker.Company.CompanyName()} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicSuppliers.Add(supplier);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllSupplierQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }
}
