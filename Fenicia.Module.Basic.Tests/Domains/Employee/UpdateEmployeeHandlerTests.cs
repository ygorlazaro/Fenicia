using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class UpdateEmployeeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new UpdateEmployeeHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private UpdateEmployeeHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenEmployeeExists_UpdatesEmployeeAndReturnsResponse()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel
        {
            Id = position1Id,
            Name = "Old Position"
        };

        var position2 = new PositionModel
        {
            Id = position2Id,
            Name = "New Position"
        };

        this.context.Positions.AddRange(position1, position2);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com",
                Document = "123.456.789-00",
                Street = "Old Street",
                Number = "100",
                ZipCode = "12345-000",
                StateId = Guid.NewGuid(),
                City = "Old City"
            }
        };

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position2Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            "Apt 202",
            "New Neighborhood",
            "200",
            Guid.NewGuid(),
            "New Street",
            "54321-000",
            "(11) 98765-4321");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Person.Name, Is.EqualTo("New Name"));
            Assert.That(result.Person.Email, Is.EqualTo("new@email.com"));
            Assert.That(result.Person.Document, Is.EqualTo("987.654.321-00"));
            Assert.That(result.PositionId, Is.EqualTo(position2Id));
        }
    }

    [Test]
    public async Task Handle_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            null,
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Phone, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Street, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.ZipCode, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Number, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Complement, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Neighborhood, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        this.context.Positions.Add(position);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
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

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.City, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesEmployeeWasUpdatedInDatabase()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel
        {
            Id = position1Id,
            Name = "Old Position"
        };

        var position2 = new PositionModel
        {
            Id = position2Id,
            Name = "New Position"
        };

        this.context.Positions.AddRange(position1, position2);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com",
                Document = "123.456.789-00",
                Street = "Old Street",
                Number = "100",
                ZipCode = "12345-000",
                StateId = Guid.NewGuid(),
                City = "Old City"
            }
        };

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position2Id,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "(11) 98765-4321",
            "New City",
            "Apt 202",
            "New Neighborhood",
            "200",
            Guid.NewGuid(),
            "New Street",
            "54321-000",
            "(11) 98765-4321");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedEmployee = await this.context.Employees
            .Include(e => e.Person)
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        Assert.That(updatedEmployee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedEmployee.Person.Name, Is.EqualTo("New Name"));
            Assert.That(updatedEmployee.Person.Email, Is.EqualTo("new@email.com"));
            Assert.That(updatedEmployee.PositionId, Is.EqualTo(position2Id));
        }
    }
}
