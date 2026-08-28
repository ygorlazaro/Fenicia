using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Equal(command.Id, result.Id);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
            CompanyId = companyId
        db.BasicEmployees.Add(employee);
        db.BasicPositions.Add(position);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            Document = faker.Random.Replace("###.###.###-##"),
            Email = faker.Internet.Email(),
            employee.Id,
    [Fact]
            faker.Internet.Email(),
        faker = new Faker();
            faker.Person.FullName,
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Employee;
            null);
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
            position.Id,
            PositionId = position.Id,
    private readonly DefaultContext db;
    private readonly EmployeeService service;
    private readonly Faker faker;
    private readonly Guid companyId;
    public async Task UpdateAsync_WhenEmployeeExists_ReturnsUpdatedEmployee()
public class UpdateEmployeeServiceTests : IDisposable
    public UpdateEmployeeServiceTests()
    public void Dispose()
        service = new EmployeeService(employeeRepository, personRepository, addressRepository, personAddressRepository, positionRepository, dashboardRepository);
        var addressRepository = new AddressRepository(db);
        var command = new UpdateEmployeeCommand(
        var companyContext = new TestCompanyContext();
        var dashboardRepository = new DashboardRepository(db);
        var employee = new EmployeeModel
        var employeeRepository = new EmployeeRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var personAddressRepository = new PersonAddressRepository(db);
        var person = new PersonModel
        var personRepository = new PersonRepository(db);
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First(), CompanyId = companyId };
        var positionRepository = new PositionRepository(db);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);
