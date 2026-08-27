using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.Handlers;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class DataSourceControllerTests : IDisposable
{
    private readonly DataSourceController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;

    public DataSourceControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var getAllPositionForDataSourceHandler = new GetAllPositionForDataSourceHandler(db);
        var getAllProductCategoryForDataSourceHandler = new GetAllProductCategoryForDataSourceHandler(db);
        var getAllSupplierForDataSourceHandler = new GetAllSupplierForDataSourceHandler(db);
        var getAllCustomerForDataSourceHandler = new GetAllCustomerForDataSourceHandler(db);
        var getAllProductForDataSourceHandler = new GetAllProductForDataSourceHandler(db);
        var getAllEmployeeForDataSourceHandler = new GetAllEmployeeForDataSourceHandler(db);

        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<GetAllPositionForDataSourceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllPositionForDataSourceQuery query, CancellationToken ct) => getAllPositionForDataSourceHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetAllProductCategoryForDataSourceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllProductCategoryForDataSourceQuery query, CancellationToken ct) => getAllProductCategoryForDataSourceHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetAllSupplierForDataSourceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllSupplierForDataSourceQuery query, CancellationToken ct) => getAllSupplierForDataSourceHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetAllCustomerForDataSourceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllCustomerForDataSourceQuery query, CancellationToken ct) => getAllCustomerForDataSourceHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetAllProductForDataSourceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllProductForDataSourceQuery query, CancellationToken ct) => getAllProductForDataSourceHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetAllEmployeeForDataSourceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllEmployeeForDataSourceQuery query, CancellationToken ct) => getAllEmployeeForDataSourceHandler.Handle(query, ct));

        mockHttpContext = new Mock<HttpContext>();

        controller = new DataSourceController(sender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims()
    {
        var claims = new List<Claim> { new("userId", Guid.NewGuid().ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetPositionsAsync_WhenNoPositionsExist_ReturnsOkWithEmptyList()
    {

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetPositionsAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.NotNull(returnedPositions);
        Assert.Empty(returnedPositions);
    }

    [Fact]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsOkWithPositions()
    {

        var position1 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        var position2 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.AddRange(position1, position2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetPositionsAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.NotNull(returnedPositions);
        Assert.Equal(2, returnedPositions.Count);
    }

    [Fact]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsPositionsOrderedByName()
    {

        var position1 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Zebra"
        };

        var position2 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Alpha"
        };

        var position3 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Manager"
        };

        db.BasicPositions.AddRange(position1, position2, position3);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetPositionsAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.NotNull(returnedPositions);
        Assert.Equal(3, returnedPositions.Count);
        Assert.Equal("Alpha", returnedPositions[0].Name);
        Assert.Equal("Manager", returnedPositions[1].Name);
        Assert.Equal("Zebra", returnedPositions[2].Name);
    }

    [Fact]
    public void DataSourceController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(DataSourceController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void DataSourceController_HasRouteAttribute()
    {

        var controllerType = typeof(DataSourceController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void DataSourceController_HasApiControllerAttribute()
    {

        var controllerType = typeof(DataSourceController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    #region Product Categories Tests

    [Fact]
    public async Task GetProductCategoriesAsync_WhenNoCategoriesExist_ReturnsOkWithEmptyList()
    {

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetProductCategoriesAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategories = okResult.Value as List<GetAllProductCategoryForDataSourceResponse>;
        Assert.NotNull(returnedCategories);
        Assert.Empty(returnedCategories);
    }

    [Fact]
    public async Task GetProductCategoriesAsync_WhenCategoriesExist_ReturnsOkWithCategories()
    {

        var category1 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Categories(1)[0]
        };

        var category2 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.AddRange(category1, category2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetProductCategoriesAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategories = okResult.Value as List<GetAllProductCategoryForDataSourceResponse>;
        Assert.NotNull(returnedCategories);
        Assert.Equal(2, returnedCategories.Count);
    }

    #endregion

    #region Suppliers Tests

    [Fact]
    public async Task GetSuppliersAsync_WhenNoSuppliersExist_ReturnsOkWithEmptyList()
    {

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetSuppliersAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSuppliers = okResult.Value as List<GetAllSupplierForDataSourceResponse>;
        Assert.NotNull(returnedSuppliers);
        Assert.Empty(returnedSuppliers);
    }

    [Fact]
    public async Task GetSuppliersAsync_WhenSuppliersExist_ReturnsOkWithSuppliers()
    {

        var person1 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName()
        };

        var person2 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Company.CompanyName()
        };

        var supplier1 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            PersonId = person1.Id,
            Person = person1
        };

        var supplier2 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            PersonId = person2.Id,
            Person = person2
        };

        db.BasicPeople.AddRange(person1, person2);
        db.BasicSuppliers.AddRange(supplier1, supplier2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetSuppliersAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSuppliers = okResult.Value as List<GetAllSupplierForDataSourceResponse>;
        Assert.NotNull(returnedSuppliers);
        Assert.Equal(2, returnedSuppliers.Count);
    }

    #endregion

    #region Customers Tests

    [Fact]
    public async Task GetCustomersAsync_WhenNoCustomersExist_ReturnsOkWithEmptyList()
    {

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetCustomersAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomers = okResult.Value as List<GetAllCustomerForDataSourceResponse>;
        Assert.NotNull(returnedCustomers);
        Assert.Empty(returnedCustomers);
    }

    [Fact]
    public async Task GetCustomersAsync_WhenCustomersExist_ReturnsOkWithCustomers()
    {

        var person1 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Name.FullName()
        };

        var person2 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Name.FullName()
        };

        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = person1.Id,
            Person = person1
        };

        var customer2 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = person2.Id,
            Person = person2
        };

        db.BasicPeople.AddRange(person1, person2);
        db.BasicCustomers.AddRange(customer1, customer2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetCustomersAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomers = okResult.Value as List<GetAllCustomerForDataSourceResponse>;
        Assert.NotNull(returnedCustomers);
        Assert.Equal(2, returnedCustomers.Count);
    }

    #endregion

    #region Products Tests

    [Fact]
    public async Task GetProductsAsync_WhenNoProductsExist_ReturnsOkWithEmptyList()
    {

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetProductsAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as List<GetAllProductForDataSourceResponse>;
        Assert.NotNull(returnedProducts);
        Assert.Empty(returnedProducts);
    }

    [Fact]
    public async Task GetProductsAsync_WhenProductsExist_ReturnsOkWithProducts()
    {

        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = 100.00m,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = 200.00m,
            CategoryId = category.Id
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetProductsAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as List<GetAllProductForDataSourceResponse>;
        Assert.NotNull(returnedProducts);
        Assert.Equal(2, returnedProducts.Count);
    }

    #endregion

    #region Employees Tests

    [Fact]
    public async Task GetEmployeesAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetEmployeesAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as List<GetAllEmployeeForDataSourceResponse>;
        Assert.NotNull(returnedEmployees);
        Assert.Empty(returnedEmployees);
    }

    [Fact]
    public async Task GetEmployeesAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Name.JobTitle()
        };

        var person1 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Name.FullName()
        };

        var person2 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Name.FullName()
        };

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PersonId = person1.Id,
            PositionId = position.Id,
            Person = person1
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PersonId = person2.Id,
            PositionId = position.Id,
            Person = person2
        };

        db.BasicPositions.Add(position);
        db.BasicPeople.AddRange(person1, person2);
        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetEmployeesAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as List<GetAllEmployeeForDataSourceResponse>;
        Assert.NotNull(returnedEmployees);
        Assert.Equal(2, returnedEmployees.Count);
    }

    #endregion
}
