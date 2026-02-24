using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.GetByPositionId;
using Fenicia.Module.Basic.Domains.Employee.Update;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.Add;
using Fenicia.Module.Basic.Domains.Position.Delete;
using Fenicia.Module.Basic.Domains.Position.GetAll;
using Fenicia.Module.Basic.Domains.Position.GetById;
using Fenicia.Module.Basic.Domains.Position.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

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
        this.testPositionId = Guid.NewGuid();
        this.getAllPositionHandler = new GetAllPositionHandler(this.context);
        this.getPositionByIdHandler = new GetPositionByIdHandler(this.context);
        this.addPositionHandler = new AddPositionHandler(this.context);
        this.updatePositionHandler = new UpdatePositionHandler(this.context);
        this.deletePositionHandler = new DeletePositionHandler(this.context);
        this.getEmployeesByPositionIdHandler = new GetEmployeesByPositionIdHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new PositionController(
            this.getAllPositionHandler,
            this.getPositionByIdHandler,
            this.addPositionHandler,
            this.updatePositionHandler,
            this.deletePositionHandler,
            this.getEmployeesByPositionIdHandler)
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
    private GetAllPositionHandler getAllPositionHandler = null!;
    private GetPositionByIdHandler getPositionByIdHandler = null!;
    private AddPositionHandler addPositionHandler = null!;
    private UpdatePositionHandler updatePositionHandler = null!;
    private DeletePositionHandler deletePositionHandler = null!;
    private GetEmployeesByPositionIdHandler getEmployeesByPositionIdHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testPositionId;
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
    public async Task GetAsync_WhenNoPositionsExist_ReturnsOkWithEmptyList()
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

        var returnedPositions = okResult.Value as List<GetAllPositionResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenPositionsExist_ReturnsOkWithPositions()
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

        this.context.Positions.AddRange(position1, position2);
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

        var returnedPositions = okResult.Value as List<GetAllPositionResponse>;
        Assert.That(returnedPositions, Is.Not.Null);
        Assert.That(returnedPositions, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsOkWithPosition()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testPositionId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPosition = okResult.Value as GetPositionByIdResponse;
        Assert.That(returnedPosition, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedPosition.Id, Is.EqualTo(this.testPositionId));
            Assert.That(returnedPosition.Name, Is.EqualTo(position.Name));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNotFound()
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
    public async Task GetEmployeesByPositionIdAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetEmployeesByPositionIdAsync(this.testPositionId, query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployees = okResult.Value as List<GetEmployeesByPositionIdResponse>;
        Assert.That(returnedEmployees, Is.Not.Null);
        Assert.That(returnedEmployees, Is.Empty);
    }

    [Test]
    public async Task GetEmployeesByPositionIdAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = this.testPositionId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = this.testPositionId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        this.context.Positions.Add(position);
        this.context.Employees.AddRange(employee1, employee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetEmployeesByPositionIdAsync(this.testPositionId, query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedEmployees = okResult.Value as List<GetEmployeesByPositionIdResponse>;
        Assert.That(returnedEmployees, Is.Not.Null);
        Assert.That(returnedEmployees, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithPosition()
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), this.faker.Commerce.Department());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedPosition = createdResult.Value as AddPositionResponse;
        Assert.That(returnedPosition, Is.Not.Null);
        Assert.That(returnedPosition.Name, Is.EqualTo(command.Name));
    }

    [Test]
    public async Task PatchAsync_WhenPositionExists_ReturnsOkWithUpdatedPosition()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(this.testPositionId, this.faker.Commerce.Department() + " Updated");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testPositionId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedPosition = okResult.Value as UpdatePositionResponse;
        Assert.That(returnedPosition, Is.Not.Null);
        Assert.That(returnedPosition.Name, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdatePositionCommand(nonExistentId, this.faker.Commerce.Department());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteAsync_WhenPositionExists_ReturnsNoContent()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testPositionId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify position was deleted
        var deletedPosition = await this.context.Positions.FirstOrDefaultAsync(x => x.Id == this.testPositionId, cancellationToken);
        Assert.That(deletedPosition, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenPositionDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void PositionController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PositionController should have Authorize attribute");
    }

    [Test]
    public void PositionController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "PositionController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void PositionController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "PositionController should have ApiController attribute");
    }
}
