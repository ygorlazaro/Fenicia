using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Bogus;
using System.Security.Claims;

namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;

public class DashboardControllerTests : IDisposable
{
    private readonly DashboardController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public DashboardControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var dashboardRepository = new DashboardRepository(db);
        var service = new DashboardService(dashboardRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new DashboardController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetFinancialDashboardAsync_ReturnsOk()
    {
        var wide = new WideEventContext();
        
        var result = await controller.GetFinancialDashboardAsync(wide, 90, CancellationToken.None);
        
        Assert.IsType<OkObjectResult>(result.Result);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
