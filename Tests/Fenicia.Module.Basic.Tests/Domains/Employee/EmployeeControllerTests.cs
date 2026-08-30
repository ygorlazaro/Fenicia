using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.Employee;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeControllerTests : IDisposable
{
    private readonly EmployeeController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TestCompanyContext _companyContext;

    public EmployeeControllerTests()
    {
        _companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, _companyContext);
        var employeeRepository = new EmployeeRepository(_db);
        var personRepository = new PersonRepository(_db);
        var addressRepository = new AddressRepository(_db);
        var personAddressRepository = new PersonAddressRepository(_db);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var stockMovementRepository = new StockMovementRepository(_db);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), orderDetailService, dummyStockMovementService);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var orderService = new OrderService(new OrderRepository(_db), orderDetailService, stockMovementService);
        var service = new EmployeeService(employeeRepository, new PersonService(personRepository), new AddressService(addressRepository), new PersonAddressService(personAddressRepository), orderService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new EmployeeController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenEmployeesExist_ReturnsOk()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsOk()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle(), CompanyId = _companyContext.CompanyId };
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName(), CompanyId = _companyContext.CompanyId };
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = position.Id, PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicPositions.Add(position);
        _db.BasicPeople.Add(person);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var result = await _controller.GetByIdAsync(employee.Id, wide, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
