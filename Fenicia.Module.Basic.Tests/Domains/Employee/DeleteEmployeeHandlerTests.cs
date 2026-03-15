using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

/// <summary>
///     Unit tests for the DeleteEmployeeHandler.
///     Tests employee deletion (soft delete) business logic.
/// </summary>
public class DeleteEmployeeHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteEmployeeHandler handler;

    public DeleteEmployeeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteEmployeeHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_SetsDeletedDate()
    {
        var employeeId = Guid.NewGuid();
        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = Guid.NewGuid(),
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

        var command = new DeleteEmployeeCommand(employeeId);
        var beforeDelete = DateTime.Now;

        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedEmployee = await db.BasicEmployees.FindAsync([employeeId], CancellationToken.None);
        Assert.NotNull(deletedEmployee);
        Assert.NotNull(deletedEmployee.Deleted);
        Assert.True(deletedEmployee.Deleted >= beforeDelete.AddSeconds(-1));
        Assert.True(deletedEmployee.Deleted <= DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var employees = await db.BasicEmployees.ToListAsync();
        Assert.Empty(employees);
    }

    [Fact]
    public async Task Handle_WithMultipleEmployees_OnlyDeletesSpecified()
    {
        // Arrange
        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();

        var employee1 = new EmployeeModel
        {
            Id = employee1Id,
            PositionId = Guid.NewGuid(),
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
            Id = employee2Id,
            PositionId = Guid.NewGuid(),
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

        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteEmployeeCommand(employee1Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedEmployee = await db.BasicEmployees.FindAsync([employee1Id], CancellationToken.None);
        var notDeletedEmployee = await db.BasicEmployees.FindAsync([employee2Id], CancellationToken.None);

        Assert.NotNull(deletedEmployee);
        Assert.NotNull(deletedEmployee.Deleted);
        Assert.NotNull(notDeletedEmployee);
        Assert.Null(notDeletedEmployee.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var employees = await db.BasicEmployees.ToListAsync();
        Assert.Empty(employees);
    }
}
