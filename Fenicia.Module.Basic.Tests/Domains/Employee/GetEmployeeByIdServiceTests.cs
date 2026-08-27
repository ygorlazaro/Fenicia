using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class GetEmployeeByIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly EmployeeService service;

    public GetEmployeeByIdServiceTests()
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
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
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

        var result = await service.GetByIdAsync(new GetEmployeeByIdQuery(employee.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var result = await service.GetByIdAsync(new GetEmployeeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }
}
