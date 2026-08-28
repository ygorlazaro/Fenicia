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
    }
{
}
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.PositionId, result.PositionId);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
        db.BasicPositions.Add(position);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
            faker.Internet.Email(),
        faker = new Faker();
            faker.Person.FullName,
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
        GC.SuppressFinalize(this);
            Guid.NewGuid(),
namespace Fenicia.Module.Basic.Tests.Domains.Employee;
            null);
            position.Id,
    private readonly DefaultContext db;
    private readonly EmployeeService service;
    private readonly Faker faker;
    private readonly Guid companyId;
    public AddEmployeeServiceTests()
    public async Task AddAsync_WithValidCommand_ReturnsAddEmployeeResponse()
public class AddEmployeeServiceTests : IDisposable
    public void Dispose()
        service = new EmployeeService(employeeRepository, personRepository, addressRepository, personAddressRepository, positionRepository, dashboardRepository);
        var addressRepository = new AddressRepository(db);
        var command = new AddEmployeeCommand(
        var companyContext = new TestCompanyContext();
        var dashboardRepository = new DashboardRepository(db);
        var employeeRepository = new EmployeeRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var personAddressRepository = new PersonAddressRepository(db);
        var personRepository = new PersonRepository(db);
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First(), CompanyId = companyId };
        var positionRepository = new PositionRepository(db);
        var result = await service.AddAsync(command, companyId, CancellationToken.None);
