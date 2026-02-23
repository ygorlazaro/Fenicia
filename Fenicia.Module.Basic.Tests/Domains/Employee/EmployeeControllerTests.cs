using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.Add;
using Fenicia.Module.Basic.Domains.Employee.Delete;
using Fenicia.Module.Basic.Domains.Employee.GetAll;
using Fenicia.Module.Basic.Domains.Employee.GetById;
using Fenicia.Module.Basic.Domains.Employee.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

[TestFixture]
public class EmployeeControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testEmployeeId = Guid.NewGuid();
        this.getAllEmployeeHandler = new GetAllEmployeeHandler(this.context);
        this.getEmployeeByIdHandler = new GetEmployeeByIdHandler(this.context);
        this.addEmployeeHandler = new AddEmployeeHandler(this.context);
        this.updateEmployeeHandler = new UpdateEmployeeHandler(this.context);
        this.deleteEmployeeHandler = new DeleteEmployeeHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new EmployeeController(
            this.getAllEmployeeHandler,
            this.getEmployeeByIdHandler,
            this.addEmployeeHandler,
            this.updateEmployeeHandler,
            this.deleteEmployeeHandler)
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

    private EmployeeController controller = null!;
    private BasicContext context = null!;
    private GetAllEmployeeHandler getAllEmployeeHandler = null!;
    private GetEmployeeByIdHandler getEmployeeByIdHandler = null!;
    private AddEmployeeHandler addEmployeeHandler = null!;
    private UpdateEmployeeHandler updateEmployeeHandler = null!;
    private DeleteEmployeeHandler deleteEmployeeHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testEmployeeId;
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
    public async Task GetAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        const int page = 1;
        const int perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployees = okResult.Value as List<EmployeeResponse>;
        Assert.That(returnedEmployees, Is.Not.Null);
        Assert.That(returnedEmployees, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####")
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####")
            }
        };

        this.context.Positions.Add(position);
        this.context.Employees.AddRange(employee1, employee2);
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

        var returnedEmployees = okResult.Value as List<EmployeeResponse>;
        Assert.That(returnedEmployees, Is.Not.Null);
        Assert.That(returnedEmployees, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsOkWithEmployee()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        var employee = new EmployeeModel
        {
            Id = this.testEmployeeId,
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####")
            }
        };

        this.context.Positions.Add(position);
        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testEmployeeId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployee = okResult.Value as EmployeeResponse;
        Assert.That(returnedEmployee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedEmployee.Id, Is.EqualTo(this.testEmployeeId));
            Assert.That(returnedEmployee.Person.Name, Is.EqualTo(employee.Person.Name));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
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
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithEmployee()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            position.Id,
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

        var returnedEmployee = createdResult.Value as EmployeeResponse;
        Assert.That(returnedEmployee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedEmployee.Person.Name, Is.EqualTo(command.Name));
            Assert.That(returnedEmployee.Person.Email, Is.EqualTo(command.Email));
        }
    }

    [Test]
    public async Task PatchAsync_WhenEmployeeExists_ReturnsOkWithUpdatedEmployee()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        var employee = new EmployeeModel
        {
            Id = this.testEmployeeId,
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####")
            }
        };

        this.context.Positions.Add(position);
        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employee.Id,
            position.Id,
            this.faker.Person.FullName + " Updated",
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
        var result = await this.controller.PatchAsync(command, this.testEmployeeId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployee = okResult.Value as EmployeeResponse;
        Assert.That(returnedEmployee, Is.Not.Null);
        Assert.That(returnedEmployee.Person.Name, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Department()
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            nonExistentId,
            position.Id,
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
    public async Task DeleteAsync_WhenEmployeeExists_ReturnsNoContent()
    {
        // Arrange
        var employee = new EmployeeModel
        {
            Id = this.testEmployeeId,
            PersonId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        this.context.Employees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testEmployeeId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NoContentResult>());

        var noContentResult = result.Result as NoContentResult;
        Assert.That(noContentResult, Is.Not.Null);
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));

        // Verify employee was deleted
        var deletedEmployee = await this.context.Employees.FirstOrDefaultAsync(x => x.Id == this.testEmployeeId, cancellationToken);
        Assert.That(deletedEmployee, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenEmployeeDoesNotExist_ReturnsNoContent()
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
    public void EmployeeController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(EmployeeController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "EmployeeController should have Authorize attribute");
    }

    [Test]
    public void EmployeeController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(EmployeeController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "EmployeeController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void EmployeeController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(EmployeeController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "EmployeeController should have ApiController attribute");
    }
}
