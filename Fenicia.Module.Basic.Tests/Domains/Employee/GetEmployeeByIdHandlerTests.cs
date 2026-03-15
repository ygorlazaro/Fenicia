using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

/// <summary>
///     Unit tests for the GetEmployeeByIdHandler.
///     Tests employee retrieval by ID logic.
/// </summary>
public class GetEmployeeByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetEmployeeByIdHandler handler;

    public GetEmployeeByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetEmployeeByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_ReturnsEmployeeResponse()
    {
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.Id);
        Assert.Equal(employee.Person.Id, result.PersonId);
        Assert.Equal(position.Id, result.PositionId);
        Assert.Equal(employee.Person.Name, result.Name);
        Assert.Equal(employee.Person.Email, result.Email);
        Assert.Equal(employee.Person.PhoneNumber, result.PhoneNumber);
        Assert.Equal(employee.Person.Document, result.Document);
        Assert.Equal(employee.Person.Street, result.Street);
        Assert.Equal(employee.Person.Number, result.Number);
        Assert.Equal(employee.Person.Complement, result.Complement);
        Assert.Equal(employee.Person.Neighborhood, result.Neighborhood);
        Assert.Equal(employee.Person.ZipCode, result.ZipCode);
        Assert.Equal(employee.Person.StateId, result.StateId);
        Assert.Equal(employee.Person.City, result.City);
    }

    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(position.Id, result.PositionId);
        Assert.Equal(employee.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleEmployees_ReturnsOnlyRequestedEmployee()
    {
        // Arrange
        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var employee1 = new EmployeeModel
        {
            Id = employee1Id,
            PositionId = position.Id,
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
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = employee2Id,
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FirstName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employee1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee1Id, result.Id);
        Assert.Equal(employee1.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                Street = string.Empty,
                Number = string.Empty,
                Complement = null,
                Neighborhood = null,
                ZipCode = string.Empty,
                StateId = state.Id,
                State = state,
                City = null,
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Person.Name, result.Name);
        Assert.Equal(employee.Person.Email, result.Email);
    }
}
