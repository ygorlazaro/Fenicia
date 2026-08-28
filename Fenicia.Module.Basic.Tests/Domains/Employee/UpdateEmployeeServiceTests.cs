using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class UpdateEmployeeServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly EmployeeService service;

    public UpdateEmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new EmployeeService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_ReturnsUpdateResponse()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            Document = faker.Random.Replace("###.###.###-##"),
            PhoneNumber = faker.Random.Replace("(##) #####-####")
        };

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Person = person,
            PersonId = person.Id
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var newName = faker.Person.FullName;
        var command = new UpdateEmployeeCommand(employee.Id, position.Id, newName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), null);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var command = new UpdateEmployeeCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), null);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.Null(result);
    }
}
