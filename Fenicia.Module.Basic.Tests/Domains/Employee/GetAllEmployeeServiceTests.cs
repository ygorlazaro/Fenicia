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
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Total);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
            CompanyId = companyId
        db.BasicEmployees.Add(employee);
        db.BasicPositions.Add(position);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            Document = faker.Random.Replace("###.###.###-##"),
            Email = faker.Internet.Email(),
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Employee;
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
            PositionId = position.Id,
    private readonly DefaultContext db;
    private readonly EmployeeService service;
    private readonly Faker faker;
    private readonly Guid companyId;
    public async Task GetAllAsync_WhenEmployeesExist_ReturnsPaginationWithEmployees()
    public async Task GetAllAsync_WhenNoEmployees_ReturnsEmptyPagination()
public class GetAllEmployeeServiceTests : IDisposable
    public GetAllEmployeeServiceTests()
    public void Dispose()
        service = new EmployeeService(employeeRepository, personRepository, addressRepository, personAddressRepository, positionRepository, dashboardRepository);
        var addressRepository = new AddressRepository(db);
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
        var result = await service.GetAllAsync(new GetAllEmployeeQuery(1, 10), CancellationToken.None);
