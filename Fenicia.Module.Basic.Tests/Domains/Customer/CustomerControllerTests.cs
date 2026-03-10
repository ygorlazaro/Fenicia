using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.Add;
using Fenicia.Module.Basic.Domains.Customer.Delete;
using Fenicia.Module.Basic.Domains.Customer.GetAll;
using Fenicia.Module.Basic.Domains.Customer.GetById;
using Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;
using Fenicia.Module.Basic.Domains.Customer.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerControllerTests : IDisposable
{
    public CustomerControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testCustomerId = Guid.NewGuid();
        var getAllCustomerHandler = new GetAllCustomerHandler(this.context);
        var getCustomerByIdHandler = new GetCustomerByIdHandler(this.context);
        var addCustomerHandler = new AddCustomerHandler(this.context);
        var updateCustomerHandler = new UpdateCustomerHandler(this.context);
        var deleteCustomerHandler = new DeleteCustomerHandler(this.context);
        var getCustomerInsightsHandler = new GetCustomerInsightsHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new CustomerController(
            getAllCustomerHandler,
            getCustomerByIdHandler,
            addCustomerHandler,
            updateCustomerHandler,
            deleteCustomerHandler,
            getCustomerInsightsHandler
            )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly CustomerController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testCustomerId;
    private readonly Faker faker;

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

    [Fact]
    public async Task GetAsync_WhenNoCustomersExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
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
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Apt 101",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        var customer2 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Apt 202",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        this.context.BasicCustomers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
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
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        this.context.AuthStates.Add(state);

        var customer = new CustomerModel
        {
            Id = this.testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Apt 101",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        this.context.BasicCustomers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testCustomerId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCustomer = okResult.Value as GetCustomerByIdResponse;
        Assert.NotNull(returnedCustomer);
        Assert.Equal(this.testCustomerId, returnedCustomer.Id);
        Assert.NotEmpty(new[] { returnedCustomer.PersonId });
        Assert.Equal(customer.Person.Name, returnedCustomer.Name);
        Assert.Equal(customer.Person.Email, returnedCustomer.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithCustomer()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedCustomer = createdResult.Value as AddCustomerResponse;
        Assert.NotNull(returnedCustomer);
        Assert.Equal(command.Id, returnedCustomer.Id);
        Assert.NotEmpty(new[] { returnedCustomer.PersonId });
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerExists_ReturnsOkWithUpdatedCustomer()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = this.testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Apt 101",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.BasicCustomers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customer.Id,
            this.faker.Person.FullName + " Updated",
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testCustomerId, wide, ct);

        // Assert
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
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(
            nonExistentId,
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.Empty,
            null,
            null,
            null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_ReturnsNoContent()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = this.testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        this.context.BasicCustomers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testCustomerId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify customer was deleted
        var deletedCustomer =
            await this.context.BasicCustomers.FirstOrDefaultAsync(x => this.testCustomerId == x.Id && x.Deleted == null, CancellationToken.None);
        Assert.Null(deletedCustomer);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void CustomerController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(CustomerController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void CustomerController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(CustomerController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void CustomerController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(CustomerController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}
