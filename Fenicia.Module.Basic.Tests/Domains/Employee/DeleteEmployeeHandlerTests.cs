using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class DeleteEmployeeHandlerTests : IDisposable
{
    public DeleteEmployeeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new DeleteEmployeeHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly DeleteEmployeeHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenEmployeeExists_SetsDeletedDate()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = Guid.NewGuid(),
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

        var command = new DeleteEmployeeCommand(employeeId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedEmployee = await this.db.BasicEmployees.FindAsync([
                employeeId
            ],
            CancellationToken.None);
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
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var employees = await this.db.BasicEmployees.ToListAsync();
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
            Id = employee2Id,
            PositionId = Guid.NewGuid(),
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

        this.db.BasicEmployees.AddRange(employee1,
            employee2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteEmployeeCommand(employee1Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedEmployee = await this.db.BasicEmployees.FindAsync([
                employee1Id
            ],
            CancellationToken.None);
        var notDeletedEmployee = await this.db.BasicEmployees.FindAsync([
                employee2Id
            ],
            CancellationToken.None);

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
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var employees = await this.db.BasicEmployees.ToListAsync();
        Assert.Empty(employees);
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
