using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Common;
using Fenicia.Module.Basic.Domains.Employee.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class UpdateEmployeeHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateEmployeeHandler handler;

    public UpdateEmployeeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateEmployeeHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_UpdatesEmployeeAndReturnsResponse()
    {
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

        db.BasicPositions.AddRange(position1, position2);

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
                Document = "12345678900",
                PhoneNumber = "11999999999",
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position2Id,
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.PersonId, result.PersonId);
        Assert.Equal(position2Id, result.PositionId);
    }

    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_ReturnsNull()
    {

        var command = new UpdateEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var command = new UpdateEmployeeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {

        var employeeId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

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
                Document = faker.Random.Replace("###########"),
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position.Id,
            "New Name",
            "new@email.com",
            "98765432100",
            null,
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_VerifiesEmployeeWasUpdatedInDatabase()
    {

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

        db.BasicPositions.AddRange(position1, position2);

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
                Document = "12345678900",
                PhoneNumber = "11999999999",
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employeeId,
            position2Id,
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        await handler.Handle(command, CancellationToken.None);

        var updatedEmployee = await db.BasicEmployees.Include(e => e.Person).FirstOrDefaultAsync(e => e.Id == employeeId);

        Assert.NotNull(updatedEmployee);
        Assert.Equal("New Name", updatedEmployee.Person.Name);
        Assert.Equal("new@email.com", updatedEmployee.Person.Email);
        Assert.Equal(position2Id, updatedEmployee.PositionId);
    }

    [Fact]
    public async Task Handle_WithAddress_CreatesOrUpdatesAddress()
    {

        var stateId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = stateId,
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);

        var employeeId = Guid.NewGuid();
        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com",
                Document = "12345678900",
                PhoneNumber = "11999999999",
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var addressDto = new AddressDTO(
            faker.Address.StreetName(),
            faker.Random.Replace("####"),
            "Apt 202",
            faker.Address.CityPrefix(),
            faker.Address.ZipCode(),
            stateId,
            faker.Address.City(),
            "Brasil"
        );

        var command = new UpdateEmployeeCommand(
            employeeId,
            employee.PositionId,
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            addressDto);

        await handler.Handle(command, CancellationToken.None);

        var address = await db.AuthAddresses.FirstOrDefaultAsync(a => a.Street == addressDto.Street);
        var personAddress = await db.BasicPersonAddresses.FirstOrDefaultAsync(pa => pa.AddressId == address!.Id);

        Assert.NotNull(address);
        Assert.NotNull(personAddress);
    }
}
