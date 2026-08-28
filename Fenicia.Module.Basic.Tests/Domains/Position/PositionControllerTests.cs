using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Fenicia.Common;
using System.Security.Claims;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionControllerTests : IDisposable
{
    private readonly PositionController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public PositionControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        
        var positionService = new PositionService(db);
        var employeeRepository = new EmployeeRepository(db);
        var personRepository = new PersonRepository(db);
        var addressRepository = new AddressRepository(db);
        var personAddressRepository = new PersonAddressRepository(db);
        var positionRepository = new PositionRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        var employeeService = new EmployeeService(employeeRepository, personRepository, addressRepository, personAddressRepository, positionRepository, dashboardRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new PositionController(positionService, employeeService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
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
    public async Task GetAsync_WhenNoPositions_ReturnsOkWithEmptyPagination()
    {
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void PositionController_HasAuthorizeAttribute()
    {
        var authorizeAttribute = typeof(PositionController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}
