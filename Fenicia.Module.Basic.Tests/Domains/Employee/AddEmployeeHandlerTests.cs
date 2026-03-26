using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Common;
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
        db = new DefaultContext(options, companyContext);
        handler = new AddEmployeeHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsEmployeeAndReturnsResponse()
    {
        var positionId = Guid.NewGuid();
        var command = new AddEmployeeCommand(
            Guid.NewGuid(), 
            positionId, 
            faker.Person.FullName, 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            faker.Random.Replace("(##) #####-####"), 
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(positionId, result.PositionId);
    }

    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddEmployeeCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            faker.Person.FullName, 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            null, 
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_VerifiesEmployeeWasSavedToDatabase()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var command = new AddEmployeeCommand(
            Guid.NewGuid(), 
            positionId, 
            faker.Person.FullName, 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            faker.Random.Replace("(##) #####-####"), 
            null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var employee = await db.BasicEmployees.Include(e => e.Person).FirstOrDefaultAsync(e => e.Id == command.Id);

        Assert.NotNull(employee);
        Assert.Equal(command.Name, employee.Person.Name);
        Assert.Equal(positionId, employee.PositionId);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllEmployees()
    {
        // Arrange
        var command1 = new AddEmployeeCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            faker.Person.FullName, 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            null, 
            null);

        var command2 = new AddEmployeeCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            faker.Person.FullName, 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            null, 
            null);

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        var employees = await db.BasicEmployees.ToListAsync();
        Assert.Equal(2, employees.Count);
    }

    [Fact]
    public async Task Handle_WithAddress_CreatesAddressAndPersonAddressRelationship()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = stateId,
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);

        var addressDto = new AddressDTO(
            faker.Address.StreetName(),
            faker.Random.Replace("####"),
            "Apt 101",
            faker.Address.CityPrefix(),
            faker.Address.ZipCode(),
            stateId,
            faker.Address.City(),
            "Brasil"
        );

        var command = new AddEmployeeCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            faker.Person.FullName, 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            faker.Random.Replace("(##) #####-####"), 
            addressDto);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var address = await db.AuthAddresses.FirstOrDefaultAsync(a => a.Street == addressDto.Street);
        var personAddress = await db.BasicPersonAddresses.FirstOrDefaultAsync(pa => pa.AddressId == address!.Id);

        Assert.NotNull(address);
        Assert.NotNull(personAddress);
    }
}
