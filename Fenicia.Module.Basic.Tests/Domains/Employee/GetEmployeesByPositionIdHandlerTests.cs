using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

/// <summary>
///     Unit tests for the GetEmployeesByPositionIdHandler.
///     Tests employee retrieval filtered by position ID.
/// </summary>
public class GetEmployeesByPositionIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetEmployeesByPositionIdHandler handler;

    public GetEmployeesByPositionIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetEmployeesByPositionIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithNoEmployeesForPosition_ReturnsEmptyList()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithEmployeesForPosition_ReturnsFilteredList()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel
        {
            Id = position1Id,
            Name = "Developer"
        };

        var position2 = new PositionModel
        {
            Id = position2Id,
            Name = "Designer"
        };

        db.BasicPositions.AddRange(position1, position2);

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = faker.Address.City()
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = faker.Address.City()
            }
        };

        var employee3 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = faker.Address.City()
            }
        };

        db.BasicEmployees.AddRange(employee1, employee2, employee3);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(position1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.All(e => e.PositionId == position1Id));
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    Street = faker.Address.StreetName(),
                    Number = faker.Random.Replace("####"),
                    ZipCode = faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = faker.Address.City()
                }
            };
            db.BasicEmployees.Add(employee);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId, 2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 5; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    Street = faker.Address.StreetName(),
                    Number = faker.Random.Replace("####"),
                    ZipCode = faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = faker.Address.City()
                }
            };
            db.BasicEmployees.Add(employee);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                PositionId = positionId,
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    Street = faker.Address.StreetName(),
                    Number = faker.Random.Replace("####"),
                    ZipCode = faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = faker.Address.City()
                }
            };
            db.BasicEmployees.Add(employee);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = positionId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = faker.Address.City()
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(employee.PersonId, result[0].PersonId);
        Assert.Equal(positionId, result[0].PositionId);
    }
}
