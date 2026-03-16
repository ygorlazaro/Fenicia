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
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithEmployeesForPosition_ReturnsFilteredList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel
        {
            Id = position1Id,
            CompanyId = companyId,
            Name = "Developer"
        };

        var position2 = new PositionModel
        {
            Id = position2Id,
            CompanyId = companyId,
            Name = "Designer"
        };

        db.BasicPositions.AddRange(position1, position2);

        var person1Id = Guid.NewGuid();
        var person2Id = Guid.NewGuid();
        var person3Id = Guid.NewGuid();

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PositionId = position1Id,
            PersonId = person1Id,
            Person = new PersonModel
            {
                Id = person1Id,
                CompanyId = companyId,
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PositionId = position1Id,
            PersonId = person2Id,
            Person = new PersonModel
            {
                Id = person2Id,
                CompanyId = companyId,
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var employee3 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PositionId = position2Id,
            PersonId = person3Id,
            Person = new PersonModel
            {
                Id = person3Id,
                CompanyId = companyId,
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.AddRange(employee1, employee2, employee3);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(position1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.True(result.Data.All(e => e.PositionId == position1Id));
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            CompanyId = companyId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var personId = Guid.NewGuid();
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PositionId = positionId,
                PersonId = personId,
                Person = new PersonModel
                {
                    Id = personId,
                    CompanyId = companyId,
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
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
        Assert.Equal(10, result.Data.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            CompanyId = companyId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 5; i++)
        {
            var personId = Guid.NewGuid();
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PositionId = positionId,
                PersonId = personId,
                Person = new PersonModel
                {
                    Id = personId,
                    CompanyId = companyId,
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
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
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            CompanyId = companyId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        for (var i = 0; i < 25; i++)
        {
            var personId = Guid.NewGuid();
            var employee = new EmployeeModel
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                PositionId = positionId,
                PersonId = personId,
                Person = new PersonModel
                {
                    Id = personId,
                    CompanyId = companyId,
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
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
        Assert.Equal(10, result.Data.Count);
    }

    [Fact]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            CompanyId = companyId,
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var personId = Guid.NewGuid();
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PositionId = positionId,
            PersonId = personId,
            Person = new PersonModel
            {
                Id = personId,
                CompanyId = companyId,
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(employee.PersonId, result.Data[0].PersonId);
        Assert.Equal(positionId, result.Data[0].PositionId);
    }
}
