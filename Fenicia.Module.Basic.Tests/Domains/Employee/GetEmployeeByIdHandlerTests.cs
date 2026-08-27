using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

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
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employeeId, result.Id);
        Assert.Equal(employee.Person.Id, result.PersonId);
        Assert.Equal(position.Id, result.PositionId);
        Assert.Equal(employee.Person.Name, result.Name);
        Assert.Equal(employee.Person.Email, result.Email);
        Assert.Equal(employee.Person.PhoneNumber, result.PhoneNumber);
        Assert.Equal(employee.Person.Document, result.Document);
    }

    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_ReturnsNull()
    {

        var query = new GetEmployeeByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetEmployeeByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesPersonAndPositionDataIsIncluded()
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
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(position.Id, result.PositionId);
        Assert.Equal(employee.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleEmployees_ReturnsOnlyRequestedEmployee()
    {

        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        db.BasicPositions.Add(position);

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
                PhoneNumber = faker.Phone.PhoneNumber(),
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
                PhoneNumber = faker.Phone.PhoneNumber(),
            }
        };

        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employee1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee1Id, result.Id);
        Assert.Equal(employee1.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithAddress_ReturnsAddressId()
    {

        var employeeId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var personId = Guid.NewGuid();
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

        var address = new AddressModel
        {
            Id = addressId,
            Street = faker.Address.StreetName(),
            Number = faker.Random.Replace("####"),
            ZipCode = faker.Address.ZipCode(),
            StateId = state.Id,
            City = faker.Address.City()
        };
        db.AuthAddresses.Add(address);

        var employee = new EmployeeModel
        {
            Id = employeeId,
            PositionId = position.Id,
            PersonId = personId,
            Person = new PersonModel
            {
                Id = personId,
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber(),
                PersonAddresses = new List<PersonAddressModel>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        PersonId = personId,
                        AddressId = addressId,
                    }
                }
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetEmployeeByIdQuery(employeeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Address);
        Assert.Equal(addressId, result.Address.Id);
    }
}
