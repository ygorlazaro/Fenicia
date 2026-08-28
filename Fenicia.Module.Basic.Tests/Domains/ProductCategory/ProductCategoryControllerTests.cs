using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Fenicia.Common;
using System.Security.Claims;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryControllerTests : IDisposable
{
    private readonly ProductCategoryController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public ProductCategoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        
        var productCategoryRepository = new ProductCategoryRepository(db);
        var productCategoryService = new ProductCategoryService(productCategoryRepository);
        var productRepository = new ProductRepository(db);
        var productCategoryRepository2 = new ProductCategoryRepository(db);
        var supplierRepository = new SupplierRepository(db);
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var productService = new ProductService(productRepository, productCategoryRepository2, supplierRepository, orderDetailRepository, stockMovementRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new ProductCategoryController(productCategoryService, productService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
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
    public async Task GetAsync_WhenNoCategories_ReturnsOkWithEmptyPagination()
    {
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void ProductCategoryController_HasAuthorizeAttribute()
    {
        var authorizeAttribute = typeof(ProductCategoryController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}
