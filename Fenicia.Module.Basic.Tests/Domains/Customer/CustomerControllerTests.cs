using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

        {
        };
    {
    }
{
}
        Assert.Empty(pagination.Data);
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(authorizeAttribute);
        Assert.NotNull(pagination);
        Assert.Single(pagination.Data);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
            CompanyId = companyId
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
        controller = new CustomerController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.BasicCustomers.Add(customer);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            Document = faker.Random.Replace("###.###.###-##"),
            Email = faker.Internet.Email(),
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
            Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Customer;
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
    private readonly CustomerController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private void SetupUserClaims(Guid userId)
    public async Task GetAsync_WhenCustomersExist_ReturnsOkWithCustomers()
    public async Task GetAsync_WhenNoCustomers_ReturnsOkWithEmptyPagination()
public class CustomerControllerTests : IDisposable
    public CustomerControllerTests()
    public void CustomerController_HasAuthorizeAttribute()
    public void Dispose()
        SetupUserClaims(testUserId);
        testUserId = Guid.NewGuid();
        var addressRepository = new AddressRepository(db);
        var authorizeAttribute = typeof(CustomerController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var customer = new CustomerModel
        var customerRepository = new CustomerRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        var okResult = result.Result as OkObjectResult;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var pagination = okResult.Value as Pagination<List<GetAllCustomerResponse>>;
        var personAddressRepository = new PersonAddressRepository(db);
        var person = new PersonModel
        var personRepository = new PersonRepository(db);
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);
        var service = new CustomerService(customerRepository, personRepository, addressRepository, personAddressRepository, dashboardRepository);
        var wide = new WideEventContext();
