using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using Fenicia.Common.Tests;
using System.Security.Claims;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Authorization;

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
        
        var service = new DashboardService(db);
        mockHttpContext = new Mock<HttpContext>();
        controller = new DashboardController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
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
    public async Task GetFinancialDashboardAsync_ReturnsOkWithDashboard()
    {
        var wide = new WideEventContext();
        var result = await controller.GetFinancialDashboardAsync(wide, 90, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void DashboardController_HasAuthorizeAttribute()
    {
        var authorizeAttribute = typeof(DashboardController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}
