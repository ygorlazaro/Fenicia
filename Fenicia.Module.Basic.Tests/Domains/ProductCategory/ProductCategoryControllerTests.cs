using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.Supplier;
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
        controller = new ProductCategoryController(productCategoryService, productService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly ProductCategoryController controller;
    private void SetupUserClaims(Guid userId)
    public async Task GetAsync_WhenNoCategories_ReturnsOkWithEmptyPagination()
public class ProductCategoryControllerTests : IDisposable
    public ProductCategoryControllerTests()
    public void Dispose()
    public void ProductCategoryController_HasAuthorizeAttribute()
        SetupUserClaims(testUserId);
        testUserId = Guid.NewGuid();
        var authorizeAttribute = typeof(ProductCategoryController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var productCategoryRepository2 = new ProductCategoryRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var productCategoryService = new ProductCategoryService(productCategoryRepository);
        var productRepository = new ProductRepository(db);
        var productService = new ProductService(productRepository, productCategoryRepository2, supplierRepository, orderDetailRepository, stockMovementRepository);
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);
        var wide = new WideEventContext();
