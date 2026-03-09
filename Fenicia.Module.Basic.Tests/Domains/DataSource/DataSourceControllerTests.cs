using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.DataSource;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

[TestFixture]
public class DataSourceControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.getAllPositionForDataSourceHandler = new GetAllPositionForDataSourceHandler(this.context);
        this.getAllProductCategoryForDataSourceHandler = new GetAllProductCategoryForDataSourceHandler(this.context);
        this.getAllSupplierForDataSourceHandler = new GetAllSupplierForDataSourceHandler(this.context);
        this.getAllCustomerForDataSourceHandler = new GetAllCustomerForDataSourceHandler(this.context);
        this.getAllProductForDataSourceHandler = new GetAllProductForDataSourceHandler(this.context);
        this.getAllEmployeeForDataSourceHandler = new GetAllEmployeeForDataSourceHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new DataSourceController(
            this.getAllPositionForDataSourceHandler,
            this.getAllProductCategoryForDataSourceHandler,
            this.getAllSupplierForDataSourceHandler,
            this.getAllCustomerForDataSourceHandler,
            this.getAllProductForDataSourceHandler,
            this.getAllEmployeeForDataSourceHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DataSourceController controller = null!;
    private DefaultContext context = null!;
    private GetAllPositionForDataSourceHandler getAllPositionForDataSourceHandler = null!;
    private GetAllProductCategoryForDataSourceHandler getAllProductCategoryForDataSourceHandler = null!;
    private GetAllSupplierForDataSourceHandler getAllSupplierForDataSourceHandler = null!;
    private GetAllCustomerForDataSourceHandler getAllCustomerForDataSourceHandler = null!;
    private GetAllProductForDataSourceHandler getAllProductForDataSourceHandler = null!;
    private GetAllEmployeeForDataSourceHandler getAllEmployeeForDataSourceHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Faker faker = null!;

    private void SetupUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId", Guid.NewGuid().ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Test]
    public async Task GetPositionsAsync_WhenNoPositionsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetPositionsAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Is.Empty);
    }

    [Test]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsOkWithPositions()
    {
        // Arrange
        var position1 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        var position2 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        this.context.BasicPositions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetPositionsAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsPositionsOrderedByName()
    {
        // Arrange
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

        this.context.BasicPositions.AddRange(position1, position2, position3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetPositionsAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPositions = okResult.Value as List<GetAllPositionForDataSourceResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPositions[0].Name, Is.EqualTo("Alpha"));
            Assert.That(returnedPositions[1].Name, Is.EqualTo("Manager"));
            Assert.That(returnedPositions[2].Name, Is.EqualTo("Zebra"));
        }
    }

    #region Product Categories Tests

    [Test]
    public async Task GetProductCategoriesAsync_WhenNoCategoriesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetProductCategoriesAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCategories = okResult.Value as List<GetAllProductCategoryForDataSourceResponse>;
        Assert.That(returnedCategories, Is.Not.Null);
        Assert.That(returnedCategories, Is.Empty);
    }

    [Test]
    public async Task GetProductCategoriesAsync_WhenCategoriesExist_ReturnsOkWithCategories()
    {
        // Arrange
        var category1 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var category2 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.BasicProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetProductCategoriesAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCategories = okResult.Value as List<GetAllProductCategoryForDataSourceResponse>;
        Assert.That(returnedCategories, Is.Not.Null);
        Assert.That(returnedCategories, Has.Count.EqualTo(2));
    }

    #endregion

    #region Suppliers Tests

    [Test]
    public async Task GetSuppliersAsync_WhenNoSuppliersExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetSuppliersAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSuppliers = okResult.Value as List<GetAllSupplierForDataSourceResponse>;
        Assert.That(returnedSuppliers, Is.Not.Null);
        Assert.That(returnedSuppliers, Is.Empty);
    }

    [Test]
    public async Task GetSuppliersAsync_WhenSuppliersExist_ReturnsOkWithSuppliers()
    {
        // Arrange
        var person1 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName()
        };

        var person2 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Company.CompanyName()
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

        this.context.BasicPeople.AddRange(person1, person2);
        this.context.BasicSuppliers.AddRange(supplier1, supplier2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetSuppliersAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSuppliers = okResult.Value as List<GetAllSupplierForDataSourceResponse>;
        Assert.That(returnedSuppliers, Is.Not.Null);
        Assert.That(returnedSuppliers, Has.Count.EqualTo(2));
    }

    #endregion

    #region Customers Tests

    [Test]
    public async Task GetCustomersAsync_WhenNoCustomersExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetCustomersAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCustomers = okResult.Value as List<GetAllCustomerForDataSourceResponse>;
        Assert.That(returnedCustomers, Is.Not.Null);
        Assert.That(returnedCustomers, Is.Empty);
    }

    [Test]
    public async Task GetCustomersAsync_WhenCustomersExist_ReturnsOkWithCustomers()
    {
        // Arrange
        var person1 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Name.FullName()
        };

        var person2 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Name.FullName()
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

        this.context.BasicPeople.AddRange(person1, person2);
        this.context.BasicCustomers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetCustomersAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCustomers = okResult.Value as List<GetAllCustomerForDataSourceResponse>;
        Assert.That(returnedCustomers, Is.Not.Null);
        Assert.That(returnedCustomers, Has.Count.EqualTo(2));
    }

    #endregion

    #region Products Tests

    [Test]
    public async Task GetProductsAsync_WhenNoProductsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetProductsAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProducts = okResult.Value as List<GetAllProductForDataSourceResponse>;
        Assert.That(returnedProducts, Is.Not.Null);
        Assert.That(returnedProducts, Is.Empty);
    }

    [Test]
    public async Task GetProductsAsync_WhenProductsExist_ReturnsOkWithProducts()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            SalesPrice = 100.00m,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            SalesPrice = 200.00m,
            CategoryId = category.Id
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetProductsAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProducts = okResult.Value as List<GetAllProductForDataSourceResponse>;
        Assert.That(returnedProducts, Is.Not.Null);
        Assert.That(returnedProducts, Has.Count.EqualTo(2));
    }

    #endregion

    #region Employees Tests

    [Test]
    public async Task GetEmployeesAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetEmployeesAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployees = okResult.Value as List<GetAllEmployeeForDataSourceResponse>;
        Assert.That(returnedEmployees, Is.Not.Null);
        Assert.That(returnedEmployees, Is.Empty);
    }

    [Test]
    public async Task GetEmployeesAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Name.JobTitle()
        };

        var person1 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Name.FullName()
        };

        var person2 = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Name.FullName()
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

        this.context.BasicPositions.Add(position);
        this.context.BasicPeople.AddRange(person1, person2);
        this.context.BasicEmployees.AddRange(employee1, employee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetEmployeesAsync(wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployees = okResult.Value as List<GetAllEmployeeForDataSourceResponse>;
        Assert.That(returnedEmployees, Is.Not.Null);
        Assert.That(returnedEmployees, Has.Count.EqualTo(2));
    }

    #endregion

    [Test]
    public void DataSourceController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(DataSourceController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "DataSourceController should have Authorize attribute");
    }

    [Test]
    public void DataSourceController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(DataSourceController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "DataSourceController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void DataSourceController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(DataSourceController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "DataSourceController should have ApiController attribute");
    }
}
