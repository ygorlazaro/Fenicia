using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement;
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
        controller = new StockMovementController(stockMovementService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly StockMovementController controller;
    private readonly StockMovementService stockMovementService;
    private void SetupUserClaims(Guid userId)
    public async Task GetAsync_WhenNoMovements_ReturnsOkWithEmptyList()
public class StockMovementControllerTests : IDisposable
    public StockMovementControllerTests()
    public void Dispose()
    public void StockMovementController_HasAuthorizeAttribute()
        SetupUserClaims(testUserId);
        stockMovementService = new StockMovementService(stockMovementRepository, productRepository);
        testUserId = Guid.NewGuid();
        var authorizeAttribute = typeof(StockMovementController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productRepository = new ProductRepository(db);
        var query = new StockMovementController.StockMovementQuery(1, 10) { StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow };
        var result = await controller.GetAsync(query, wide, CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);
        var wide = new WideEventContext();
