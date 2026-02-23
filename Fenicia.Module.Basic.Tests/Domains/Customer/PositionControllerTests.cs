using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.Add;
using Fenicia.Module.Basic.Domains.Customer.Delete;
using Fenicia.Module.Basic.Domains.Customer.GetAll;
using Fenicia.Module.Basic.Domains.Customer.GetById;
using Fenicia.Module.Basic.Domains.Customer.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class PositionControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testCustomerId = Guid.NewGuid();
        this.getAllCustomerHandler = new GetAllCustomerHandler(this.context);
        this.getCustomerByIdHandler = new GetCustomerByIdHandler(this.context);
        this.addCustomerHandler = new AddCustomerHandler(this.context);
        this.updateCustomerHandler = new UpdateCustomerHandler(this.context);
        this.deleteCustomerHandler = new DeleteCustomerHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new PositionController(
            this.getAllCustomerHandler,
            this.getCustomerByIdHandler,
            this.addCustomerHandler,
            this.updateCustomerHandler,
            this.deleteCustomerHandler)
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

    private PositionController controller = null!;
    private BasicContext context = null!;
    private GetAllCustomerHandler getAllCustomerHandler = null!;
    private GetCustomerByIdHandler getCustomerByIdHandler = null!;
    private AddCustomerHandler addCustomerHandler = null!;
    private UpdateCustomerHandler updateCustomerHandler = null!;
    private DeleteCustomerHandler deleteCustomerHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testCustomerId;
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
    public async Task GetAsync_WhenNoCustomersExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var page = 1;
        var perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCustomers = okResult.Value as List<CustomerResponse>;
        Assert.That(returnedCustomers, Is.Not.Null);
        Assert.That(returnedCustomers, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenCustomersExist_ReturnsOkWithCustomers()
    {
        // Arrange
        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Cpf = this.faker.Random.Replace("###.###.###-##"),
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

        var customer2 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Cpf = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Apt 202",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.AddRange(customer1, customer2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCustomers = okResult.Value as List<CustomerResponse>;
        Assert.That(returnedCustomers, Is.Not.Null);
        Assert.That(returnedCustomers, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsOkWithCustomer()
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
                Cpf = this.faker.Random.Replace("###.###.###-##"),
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

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testCustomerId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCustomer = okResult.Value as CustomerResponse;
        Assert.That(returnedCustomer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedCustomer.Id, Is.EqualTo(this.testCustomerId));
            Assert.That(returnedCustomer.Person.Name, Is.EqualTo(customer.Person.Name));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithCustomer()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedCustomer = createdResult.Value as CustomerResponse;
        Assert.That(returnedCustomer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedCustomer.Person.Name, Is.EqualTo(command.Name));
            Assert.That(returnedCustomer.Person.Email, Is.EqualTo(command.Email));
        }
    }

    [Test]
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
                Cpf = this.faker.Random.Replace("###.###.###-##"),
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

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customer.Id,
            this.faker.Person.FullName + " Updated",
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testCustomerId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCustomer = okResult.Value as CustomerResponse;
        Assert.That(returnedCustomer, Is.Not.Null);
        Assert.That(returnedCustomer.Person.Name, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(
            nonExistentId,
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.Empty,
            null,
            null,
            null);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
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
                Cpf = this.faker.Random.Replace("###.###.###-##")
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testCustomerId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NoContentResult>());

        var noContentResult = result.Result as NoContentResult;
        Assert.That(noContentResult, Is.Not.Null);
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));

        // Verify customer was deleted
        var deletedCustomer =
            await this.context.Customers.FirstOrDefaultAsync(x => this.testCustomerId == x.Id, CancellationToken.None);
        Assert.That(deletedCustomer, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public void CustomerController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "CustomerController should have Authorize attribute");
    }

    [Test]
    public void CustomerController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "CustomerController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void CustomerController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "CustomerController should have ApiController attribute");
    }
}