using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Dashboard;
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
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
        controller = new DashboardController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;
    private readonly DashboardController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private void SetupUserClaims(Guid userId)
    public async Task GetFinancialDashboardAsync_ReturnsOk()
public class DashboardControllerTests : IDisposable
    public DashboardControllerTests()
    public void Dispose()
        SetupUserClaims(testUserId);
        testUserId = Guid.NewGuid();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var dashboardRepository = new DashboardRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await controller.GetFinancialDashboardAsync(wide, 90, CancellationToken.None);
        var service = new DashboardService(dashboardRepository);
        var wide = new WideEventContext();
