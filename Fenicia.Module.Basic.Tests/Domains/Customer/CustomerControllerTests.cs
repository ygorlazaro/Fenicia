using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;
using Fenicia.Module.Basic.Domains.Customer.Queries;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerControllerTests : IDisposable
{
    private readonly CustomerController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testCustomerId;

    public CustomerControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testCustomerId = Guid.NewGuid();

        var getAllCustomerHandler = new GetAllCustomerHandler(db);
        var getCustomerByIdHandler = new GetCustomerByIdHandler(db);
        var addCustomerHandler = new AddCustomerHandler(db);
        var updateCustomerHandler = new UpdateCustomerHandler(db);
        var deleteCustomerHandler = new DeleteCustomerHandler(db);
        var getCustomerInsightsHandler = new GetCustomerInsightsHandler(db);

        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<GetAllCustomerQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllCustomerQuery query, CancellationToken ct) => getAllCustomerHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetCustomerByIdQuery query, CancellationToken ct) => getCustomerByIdHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<AddCustomerCommand>(), It.IsAny<CancellationToken>()))
            .Returns((AddCustomerCommand command, CancellationToken ct) => addCustomerHandler.Handle(command, ct));
        sender.Setup(x => x.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .Returns((UpdateCustomerCommand command, CancellationToken ct) => updateCustomerHandler.Handle(command, ct));
        sender.Setup(x => x.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
            .Returns((DeleteCustomerCommand command, CancellationToken ct) => deleteCustomerHandler.Handle(command, ct));
        sender.Setup(x => x.Send(It.IsAny<GetCustomerInsightsQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetCustomerInsightsQuery query, CancellationToken ct) => getCustomerInsightsHandler.Handle(query, ct));

        mockHttpContext = new Mock<HttpContext>();

        controller = new CustomerController(sender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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
    public async Task GetAsync_WhenNoCustomersExist_ReturnsOkWithEmptyList()
    {
        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomers = okResult.Value as Pagination<List<GetAllCustomerResponse>>;
        Assert.NotNull(returnedCustomers);
        Assert.Empty(returnedCustomers.Data);
        Assert.Equal(0, returnedCustomers.Total);
    }

    [Fact]
    public async Task GetAsync_WhenCustomersExist_ReturnsOkWithCustomers()
    {
        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                CompanyId = Guid.NewGuid()
            }
        };

        var customer2 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                CompanyId = Guid.NewGuid()
            }
        };

        db.BasicCustomers.AddRange(customer1, customer2);
        await db.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomers = okResult.Value as Pagination<List<GetAllCustomerResponse>>;
        Assert.NotNull(returnedCustomers);
        Assert.Equal(2, returnedCustomers.Data.Count);
        Assert.Equal(2, returnedCustomers.Total);
        Assert.Equal(customer1.Person.Name, returnedCustomers.Data[0].Name);
        Assert.Equal(customer1.Person.Email, returnedCustomers.Data[0].Email);
        Assert.Equal(customer2.Person.Name, returnedCustomers.Data[1].Name);
        Assert.Equal(customer2.Person.Email, returnedCustomers.Data[1].Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsOkWithCustomer()
    {
        var customer = new CustomerModel
        {
            Id = testCustomerId,
            PersonId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                CompanyId = Guid.NewGuid()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testCustomerId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomer = okResult.Value as GetCustomerByIdResponse;
        Assert.NotNull(returnedCustomer);
        Assert.Equal(testCustomerId, returnedCustomer.Id);
        Assert.NotEmpty(new[] { returnedCustomer.PersonId });
        Assert.Equal(customer.Person.Name, returnedCustomer.Name);
        Assert.Equal(customer.Person.Email, returnedCustomer.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithCustomer()
    {
        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedCustomer = createdResult.Value as AddCustomerResponse;
        Assert.NotNull(returnedCustomer);
        Assert.NotEmpty(new[] { returnedCustomer.Id });
        Assert.NotEmpty(new[] { returnedCustomer.PersonId });
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerExists_ReturnsOkWithUpdatedCustomer()
    {
        var customer = new CustomerModel
        {
            Id = testCustomerId,
            PersonId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                CompanyId = Guid.NewGuid()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customer.Id,
            faker.Person.FullName + " Updated",
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testCustomerId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomer = okResult.Value as UpdateCustomerResponse;
        Assert.NotNull(returnedCustomer);
        Assert.Equal(command.Id, returnedCustomer.Id);
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(
            nonExistentId,
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_ReturnsNoContent()
    {
        var customer = new CustomerModel
        {
            Id = testCustomerId,
            PersonId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                CompanyId = Guid.NewGuid()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testCustomerId, wide, ct);

        Assert.NotNull(result);

        var deletedCustomer = await db.BasicCustomers.FirstOrDefaultAsync(x => testCustomerId == x.Id && x.Deleted == null, CancellationToken.None);
        Assert.Null(deletedCustomer);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ReturnsNoContent()
    {
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
    }

    [Fact]
    public void CustomerController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(CustomerController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void CustomerController_HasRouteAttribute()
    {
        var controllerType = typeof(CustomerController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void CustomerController_HasApiControllerAttribute()
    {
        var controllerType = typeof(CustomerController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }
}
