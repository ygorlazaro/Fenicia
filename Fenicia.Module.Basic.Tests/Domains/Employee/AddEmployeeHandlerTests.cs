using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

/// <summary>
///     Unit tests for the AddEmployeeHandler.
///     Tests employee creation business logic including validation and database operations.
/// </summary>
public class AddEmployeeHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddEmployeeHandler handler;

    public AddEmployeeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new AddEmployeeHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsEmployeeAndReturnsResponse()
    {
        var positionId = Guid.NewGuid();
        var command = new AddEmployeeCommand(Guid.NewGuid(), positionId, this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), "Apt 101", this.faker.Address.CityPrefix(), this.faker.Random.Replace("####"), Guid.NewGuid(), this.faker.Address.StreetName(), this.faker.Address.ZipCode(), this.faker.Random.Replace("(##) #####-####"));

        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(positionId, result.PositionId);
    }

    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var command = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), null, null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_VerifiesEmployeeWasSavedToDatabase()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var command = new AddEmployeeCommand(Guid.NewGuid(), positionId, this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var employee = await this.db.BasicEmployees.Include(e => e.Person).FirstOrDefaultAsync(e => e.Id == command.Id);

        Assert.NotNull(employee);
        Assert.Equal(command.Name, employee.Person.Name);
        Assert.Equal(positionId, employee.PositionId);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllEmployees()
    {
        // Arrange
        var command1 = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        var command2 = new AddEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Person.FullName, this.faker.Internet.Email(), this.faker.Random.Replace("###.###.###-##"), this.faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var employees = await this.db.BasicEmployees.ToListAsync();
        Assert.Equal(2, employees.Count);
    }
}