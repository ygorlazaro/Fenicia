using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.DataSource;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
        await db.SaveChangesAsync(CancellationToken.None);
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
namespace Fenicia.Module.Basic.Tests.Domains.DataSource;
            PersonId = person.Id
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####")
            PositionId = position.Id,
    private readonly DataSourceService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    public async Task GetEmployeesAsync_WhenEmployeesExist_ReturnsEmployees()
    public async Task GetEmployeesAsync_WhenNoEmployees_ReturnsEmptyList()
public class GetAllEmployeeForDataSourceServiceTests : IDisposable
    public GetAllEmployeeForDataSourceServiceTests()
    public void Dispose()
        service = new DataSourceService(db);
        var companyContext = new TestCompanyContext();
        var employee = new EmployeeModel
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var person = new PersonModel
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var result = await service.GetEmployeesAsync(CancellationToken.None);
