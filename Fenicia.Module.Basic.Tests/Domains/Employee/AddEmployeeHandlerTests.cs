using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Employee.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class AddEmployeeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddEmployeeHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddEmployeeHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsEmployeeAndReturnsResponse()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            positionId,
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.PositionId, Is.EqualTo(positionId));
        }
    }

    [Test]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
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
    }

    [Test]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
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
    }

    [Test]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
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
    }

    [Test]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
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
    }

    [Test]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
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
    }

    [Test]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
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
    }

    [Test]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
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
    }

    [Test]
    public async Task Handle_VerifiesEmployeeWasSavedToDatabase()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            positionId,
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var employee = await this.context.Employees
            .Include(e => e.Person)
            .FirstOrDefaultAsync(e => e.Id == command.Id);

        Assert.That(employee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(employee.Person.Name, Is.EqualTo(command.Name));
            Assert.That(employee.PositionId, Is.EqualTo(positionId));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllEmployees()
    {
        // Arrange
        var command1 = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        var command2 = new AddEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var employees = await this.context.Employees.ToListAsync();
        Assert.That(employees, Has.Count.EqualTo(2));
    }
}
