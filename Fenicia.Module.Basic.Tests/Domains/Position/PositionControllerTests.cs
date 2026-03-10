using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.GetByPositionId;
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

public class PositionControllerTests : IDisposable
{
    public PositionControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testPositionId = Guid.NewGuid();
        var getAllPositionHandler = new GetAllPositionHandler(this.context);
        var getPositionByIdHandler = new GetPositionByIdHandler(this.context);
        var addPositionHandler = new AddPositionHandler(this.context);
        var updatePositionHandler = new UpdatePositionHandler(this.context);
        var deletePositionHandler = new DeletePositionHandler(this.context);
        var getEmployeesByPositionIdHandler = new GetEmployeesByPositionIdHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new PositionController(
            getAllPositionHandler,
            getPositionByIdHandler,
            addPositionHandler,
            updatePositionHandler,
            deletePositionHandler,
            getEmployeesByPositionIdHandler)
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

    private readonly PositionController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testPositionId;
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
    public async Task GetAsync_WhenNoPositionsExist_ReturnsOkWithEmptyList()
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

        var returnedPositions = okResult.Value as Pagination<List<GetAllPositionResponse>>;
        Assert.NotNull(returnedPositions);
        Assert.Empty(returnedPositions.Data);
        Assert.Equal(0, returnedPositions.Total);
    }

    [Fact]
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

        this.context.BasicPositions.AddRange(position1, position2);
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

        var returnedPositions = okResult.Value as Pagination<List<GetAllPositionResponse>>;
        Assert.NotNull(returnedPositions);
        Assert.Equal(2, returnedPositions.Data.Count);
        Assert.Equal(2, returnedPositions.Total);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsOkWithPosition()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testPositionId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPosition = okResult.Value as GetPositionByIdResponse;
        Assert.NotNull(returnedPosition);
        Assert.Equal(this.testPositionId, returnedPosition.Id);
        Assert.Equal(position.Name, returnedPosition.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNotFound()
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
    public async Task GetEmployeesByPositionIdAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetEmployeesByPositionIdAsync(this.testPositionId, query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as List<GetEmployeesByPositionIdResponse>;
        Assert.NotNull(returnedEmployees);
        Assert.Empty(returnedEmployees);
    }

    [Fact]
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

        this.context.BasicPositions.Add(position);
        this.context.BasicEmployees.AddRange(employee1, employee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetEmployeesByPositionIdAsync(this.testPositionId, query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as List<GetEmployeesByPositionIdResponse>;
        Assert.NotNull(returnedEmployees);
        Assert.Equal(2, returnedEmployees.Count);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithPosition()
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), this.faker.Commerce.Department());
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

        var returnedPosition = createdResult.Value as AddPositionResponse;
        Assert.NotNull(returnedPosition);
        Assert.Equal(command.Name, returnedPosition.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenPositionExists_ReturnsOkWithUpdatedPosition()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(this.testPositionId, this.faker.Commerce.Department() + " Updated");
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testPositionId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPosition = okResult.Value as UpdatePositionResponse;
        Assert.NotNull(returnedPosition);
        Assert.Contains("Updated", returnedPosition.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdatePositionCommand(nonExistentId, this.faker.Commerce.Department());
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_ReturnsNoContent()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = this.testPositionId,
            Name = this.faker.Commerce.Department()
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testPositionId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify position was deleted
        var deletedPosition = await this.context.BasicPositions.FirstOrDefaultAsync(x => x.Id == this.testPositionId && x.Deleted == null, ct);
        Assert.Null(deletedPosition);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionDoesNotExist_ReturnsNoContent()
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
    public void PositionController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void PositionController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void PositionController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(PositionController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}
