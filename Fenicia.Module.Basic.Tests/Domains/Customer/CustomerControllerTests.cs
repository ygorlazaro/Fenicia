using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Fenicia.Common;
using System.Security.Claims;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerControllerTests : IDisposable
{
    private readonly CustomerController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly Guid companyId;

    public CustomerControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var customerRepository = new CustomerRepository(db);
        var personRepository = new PersonRepository(db);
        var addressRepository = new AddressRepository(db);
        var personAddressRepository = new PersonAddressRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        var service = new CustomerService(customerRepository, personRepository, addressRepository, personAddressRepository, dashboardRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new CustomerController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
        companyId = companyContext.CompanyId;
        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenNoCustomers_ReturnsOkWithEmptyPagination()
    {
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        var pagination = okResult.Value as Pagination<List<GetAllCustomerResponse>>;
        Assert.NotNull(pagination);
        Assert.Empty(pagination.Data);
    }

    [Fact]
    public async Task GetAsync_WhenCustomersExist_ReturnsOkWithCustomers()
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            Document = faker.Random.Replace("###.###.###-##"),
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
            CompanyId = companyId
        };

        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            Person = person,
            PersonId = person.Id,
            CompanyId = companyId
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        var pagination = okResult.Value as Pagination<List<GetAllCustomerResponse>>;
        Assert.NotNull(pagination);
        Assert.Single(pagination.Data);
    }

    [Fact]
    public void CustomerController_HasAuthorizeAttribute()
    {
        var authorizeAttribute = typeof(CustomerController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}
