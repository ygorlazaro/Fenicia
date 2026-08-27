using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class GetAllEmployeeHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllEmployeeHandler handler;

    public GetAllEmployeeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllEmployeeHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {

        var query = new GetAllEmployeeQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithEmployees_ReturnsAllEmployees()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(employee1.Person.Id, result.Data[0].PersonId);
        Assert.Equal(employee1.Person.Name, result.Data[0].Name);
        Assert.Equal(position.Name, result.Data[0].PositionName);

        Assert.Equal(employee2.Person.Id, result.Data[1].PersonId);
        Assert.Equal(employee2.Person.Name, result.Data[1].Name);
        Assert.Equal(position.Name, result.Data[1].PositionName);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicEmployees.Add(employee);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 5; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicEmployees.Add(employee);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = position.Id,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicEmployees.Add(employee);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
    }

    [Fact]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllEmployeeQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(employee.Person.Id, result.Data[0].PersonId);
        Assert.Equal(position.Id, result.Data[0].PositionId);
        Assert.Equal(employee.Person.Name, result.Data[0].Name);
        Assert.Equal(position.Name, result.Data[0].PositionName);
    }
}
