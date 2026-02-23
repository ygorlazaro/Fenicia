using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class DeleteEmployeeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new DeleteEmployeeHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private DeleteEmployeeHandler handler = null!;
    private Faker faker = null!;

    [Test]
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
                Cpf = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteEmployeeCommand(employeeId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedEmployee = await this.context.Employees.FindAsync([employeeId], CancellationToken.None);
        Assert.That(deletedEmployee, Is.Not.Null);
        Assert.That(deletedEmployee.Deleted, Is.Not.Null);
        Assert.That(deletedEmployee.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedEmployee.Deleted, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenEmployeeDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var employees = await this.context.Employees.ToListAsync();
        Assert.That(employees, Is.Empty);
    }

    [Test]
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
                Cpf = this.faker.Random.Replace("###.###.###-##"),
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
                Cpf = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Employees.AddRange(employee1, employee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteEmployeeCommand(employee1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedEmployee = await this.context.Employees.FindAsync([employee1Id], CancellationToken.None);
        var notDeletedEmployee = await this.context.Employees.FindAsync([employee2Id], CancellationToken.None);

        Assert.That(deletedEmployee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedEmployee.Deleted, Is.Not.Null);
            Assert.That(notDeletedEmployee, Is.Not.Null);
        }
        Assert.That(notDeletedEmployee?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteEmployeeCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var employees = await this.context.Employees.ToListAsync();
        Assert.That(employees, Is.Empty);
    }
}
