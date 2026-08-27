using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Handlers;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionControllerTests : IDisposable
{
    private readonly PositionController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<ISender> mockSender;
    private readonly Guid testPositionId;

    public PositionControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testPositionId = Guid.NewGuid();
        var getAllPositionHandler = new GetAllPositionHandler(db);
        var getPositionByIdHandler = new GetPositionByIdHandler(db);
        var addPositionHandler = new AddPositionHandler(db);
        var updatePositionHandler = new UpdatePositionHandler(db);
        var deletePositionHandler = new DeletePositionHandler(db);
        var getEmployeesByPositionIdHandler = new GetEmployeesByPositionIdHandler(db);
        mockSender = new Mock<ISender>();
        mockHttpContext = new Mock<HttpContext>();

        mockSender.Setup(s => s.Send(It.IsAny<GetAllPositionQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllPositionQuery query, CancellationToken ct) => getAllPositionHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetPositionByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetPositionByIdQuery query, CancellationToken ct) => getPositionByIdHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetEmployeesByPositionIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetEmployeesByPositionIdQuery query, CancellationToken ct) => getEmployeesByPositionIdHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<AddPositionCommand>(), It.IsAny<CancellationToken>()))
            .Returns((AddPositionCommand command, CancellationToken ct) => addPositionHandler.Handle(command, ct));

        mockSender.Setup(s => s.Send(It.IsAny<UpdatePositionCommand>(), It.IsAny<CancellationToken>()))
            .Returns((UpdatePositionCommand command, CancellationToken ct) => updatePositionHandler.Handle(command, ct));

        mockSender.Setup(s => s.Send(It.IsAny<DeletePositionCommand>(), It.IsAny<CancellationToken>()))
            .Returns((DeletePositionCommand command, CancellationToken ct) => deletePositionHandler.Handle(command, ct));

        controller = new PositionController(mockSender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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
    public async Task GetAsync_WhenNoPositionsExist_ReturnsOkWithEmptyList()
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

        var returnedPositions = okResult.Value as Pagination<List<GetAllPositionResponse>>;
        Assert.NotNull(returnedPositions);
        Assert.Empty(returnedPositions.Data);
        Assert.Equal(0, returnedPositions.Total);
    }

    [Fact]
    public async Task GetAsync_WhenPositionsExist_ReturnsOkWithPositions()
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

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

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

        var position = new PositionModel
        {
            Id = testPositionId,
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testPositionId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedPosition = okResult.Value as GetPositionByIdResponse;
        Assert.NotNull(returnedPosition);
        Assert.Equal(testPositionId, returnedPosition.Id);
        Assert.Equal(position.Name, returnedPosition.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetEmployeesByPositionIdAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {

        var position = new PositionModel
        {
            Id = testPositionId,
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetEmployeesByPositionIdAsync(testPositionId, query, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as Pagination<List<GetEmployeesByPositionIdResponse>>;
        Assert.NotNull(returnedEmployees);
        Assert.Empty(returnedEmployees.Data);
    }

    [Fact]
    public async Task GetEmployeesByPositionIdAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {

        var position = new PositionModel
        {
            Id = testPositionId,
            Name = faker.Commerce.Department()
        };

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = testPositionId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        var employee2 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = testPositionId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        db.BasicPositions.Add(position);
        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetEmployeesByPositionIdAsync(testPositionId, query, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as Pagination<List<GetEmployeesByPositionIdResponse>>;
        Assert.NotNull(returnedEmployees);
        Assert.Equal(2, returnedEmployees.Data.Count);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithPosition()
    {

        var command = new AddPositionCommand(Guid.NewGuid(), faker.Commerce.Department());
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

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

        var position = new PositionModel
        {
            Id = testPositionId,
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(testPositionId, faker.Commerce.Department() + " Updated");
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testPositionId, wide, ct);

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

        var nonExistentId = Guid.NewGuid();
        var command = new UpdatePositionCommand(nonExistentId, faker.Commerce.Department());
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_ReturnsNoContent()
    {

        var position = new PositionModel
        {
            Id = testPositionId,
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testPositionId, wide, ct);

        Assert.NotNull(result);

        var deletedPosition = await db.BasicPositions.FirstOrDefaultAsync(x => x.Id == testPositionId && x.Deleted == null, ct);
        Assert.Null(deletedPosition);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionDoesNotExist_ReturnsNoContent()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
    }

    [Fact]
    public void PositionController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(PositionController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void PositionController_HasRouteAttribute()
    {

        var controllerType = typeof(PositionController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void PositionController_HasApiControllerAttribute()
    {

        var controllerType = typeof(PositionController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }
}
