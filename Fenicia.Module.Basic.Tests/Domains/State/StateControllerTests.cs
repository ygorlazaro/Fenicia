using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State;
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
        controller = new StateController(stateService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
namespace Fenicia.Module.Basic.Tests.Domains.State;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly StateController controller;
    private readonly StateService stateService;
    private void SetupUserClaims(Guid userId)
    public async Task GetAllAsync_WhenNoStates_ReturnsOkWithEmptyList()
public class StateControllerTests : IDisposable
    public StateControllerTests()
    public void Dispose()
    public void StateController_HasAuthorizeAttribute()
        SetupUserClaims(testUserId);
        stateService = new StateService(stateRepository);
        testUserId = Guid.NewGuid();
        var authorizeAttribute = typeof(StateController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await controller.GetAllAsync(wide, CancellationToken.None);
        var stateRepository = new StateRepository(db);
        var wide = new WideEventContext();
