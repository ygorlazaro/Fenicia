using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class GetEmployeesByPositionIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly EmployeeService service;
    private readonly Guid companyId;

    public GetEmployeesByPositionIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var employeeRepository = new EmployeeRepository(db);
        var personRepository = new PersonRepository(db);
        var addressRepository = new AddressRepository(db);
        var personAddressRepository = new PersonAddressRepository(db);
        var positionRepository = new PositionRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        service = new EmployeeService(employeeRepository, personRepository, addressRepository, personAddressRepository, positionRepository, dashboardRepository);
        faker = new Faker();
        companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByPositionIdAsync_ReturnsEmployeesByPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First(), CompanyId = companyId };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            Document = faker.Random.Replace("###.###.###-##"),
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
            CompanyId = companyId
        };

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Person = person,
            PersonId = person.Id,
            CompanyId = companyId
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByPositionIdAsync(new GetEmployeesByPositionIdQuery(position.Id, 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
    }
}
