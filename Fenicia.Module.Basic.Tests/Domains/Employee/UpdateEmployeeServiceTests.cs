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

public class UpdateEmployeeServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly EmployeeService service;
    private readonly Guid companyId;

    public UpdateEmployeeServiceTests()
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
    public async Task UpdateAsync_WhenEmployeeExists_ReturnsUpdatedEmployee()
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

        var command = new UpdateEmployeeCommand(
            employee.Id,
            position.Id,
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
    }
}
