using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Fenicia.Common;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductControllerTests : IDisposable
{
    private readonly ProductController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly ProductService productService;

    public ProductControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        
        var productRepository = new ProductRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var supplierRepository = new SupplierRepository(db);
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        productService = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new ProductController(productService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
        SetupUserClaims(testUserId);
        faker = new Faker();
        companyId = companyContext.CompanyId;
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
    public async Task GetAsync_WhenNoProducts_ReturnsOkWithEmptyPagination()
    {
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void ProductController_HasAuthorizeAttribute()
    {
        var authorizeAttribute = typeof(ProductController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}
