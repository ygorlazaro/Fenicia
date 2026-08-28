using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Domains.Position;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

        
    {
    }
{
}
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(authorizeAttribute);
        companyId = companyContext.CompanyId;
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
        controller = new PositionController(positionService, employeeService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
namespace Fenicia.Module.Basic.Tests.Domains.Position;
        positionService = new PositionService(positionRepository);
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly PositionController controller;
    private readonly PositionService positionService;
    private void SetupUserClaims(Guid userId)
    public async Task GetAsync_WhenNoPositions_ReturnsOkWithEmptyPagination()
public class PositionControllerTests : IDisposable
    public PositionControllerTests()
    public void Dispose()
    public void PositionController_HasAuthorizeAttribute()
        SetupUserClaims(testUserId);
        testUserId = Guid.NewGuid();
        var addressRepository = new AddressRepository(db);
        var authorizeAttribute = typeof(PositionController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var dashboardRepository = new DashboardRepository(db);
        var employeeRepository = new EmployeeRepository(db);
        var employeeService = new EmployeeService(employeeRepository, personRepository, addressRepository, personAddressRepository, positionRepository, dashboardRepository);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var personAddressRepository = new PersonAddressRepository(db);
        var personRepository = new PersonRepository(db);
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);
        var wide = new WideEventContext();
