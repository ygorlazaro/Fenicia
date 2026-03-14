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
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetEmployeesByPositionIdHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithNoEmployeesForPosition_ReturnsEmptyList()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

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

        var position1 = new PositionModel { Id = position1Id, Name = "Developer" };

        var position2 = new PositionModel { Id = position2Id, Name = "Designer" };

        this.db.BasicPositions.AddRange(position1, position2);

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
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
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
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
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.db.BasicEmployees.AddRange(employee1, employee2, employee3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(position1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

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
        var position = new PositionModel { Id = positionId, Name = "Developer" };
        this.db.BasicPositions.Add(position);

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
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = this.faker.Address.City()
                }
            };
            this.db.BasicEmployees.Add(employee);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId, 2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel { Id = positionId, Name = "Developer" };
        this.db.BasicPositions.Add(position);

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
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = this.faker.Address.City()
                }
            };
            this.db.BasicEmployees.Add(employee);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel { Id = positionId, Name = "Developer" };
        this.db.BasicPositions.Add(position);

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
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = Guid.NewGuid(),
                    City = this.faker.Address.City()
                }
            };
            this.db.BasicEmployees.Add(employee);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel { Id = positionId, Name = "Developer" };
        this.db.BasicPositions.Add(position);

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = positionId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.db.BasicEmployees.Add(employee);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeesByPositionIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(employee.PersonId, result[0].PersonId);
        Assert.Equal(positionId, result[0].PositionId);
    }
}