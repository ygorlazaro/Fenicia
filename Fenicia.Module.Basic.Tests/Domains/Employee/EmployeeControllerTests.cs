using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeControllerTests : IDisposable
{
    private readonly EmployeeController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<ISender> mockSender;
    private readonly Guid testEmployeeId;

    public EmployeeControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testEmployeeId = Guid.NewGuid();
        var getAllEmployeeHandler = new GetAllEmployeeHandler(db);
        var getEmployeeByIdHandler = new GetEmployeeByIdHandler(db);
        var addEmployeeHandler = new AddEmployeeHandler(db);
        var updateEmployeeHandler = new UpdateEmployeeHandler(db);
        var deleteEmployeeHandler = new DeleteEmployeeHandler(db);
        var getEmployeePerformanceHandler = new GetEmployeePerformanceHandler(db);
        mockSender = new Mock<ISender>();
        mockHttpContext = new Mock<HttpContext>();

        mockSender.Setup(s => s.Send(It.IsAny<GetAllEmployeeQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAllEmployeeQuery query, CancellationToken ct) => getAllEmployeeHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetEmployeeByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetEmployeeByIdQuery query, CancellationToken ct) => getEmployeeByIdHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<AddEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .Returns((AddEmployeeCommand command, CancellationToken ct) => addEmployeeHandler.Handle(command, ct));

        mockSender.Setup(s => s.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .Returns((UpdateEmployeeCommand command, CancellationToken ct) => updateEmployeeHandler.Handle(command, ct));

        mockSender.Setup(s => s.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .Returns((DeleteEmployeeCommand command, CancellationToken ct) => deleteEmployeeHandler.Handle(command, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetEmployeePerformanceQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetEmployeePerformanceQuery query, CancellationToken ct) => getEmployeePerformanceHandler.Handle(query, ct));

        controller = new EmployeeController(mockSender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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
    public async Task GetAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
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

        var returnedEmployees = okResult.Value as Pagination<List<GetAllEmployeeResponse>>;
        Assert.NotNull(returnedEmployees);
        Assert.Empty(returnedEmployees.Data);
        Assert.Equal(0, returnedEmployees.Total);
    }

    [Fact]
    public async Task GetAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        var employee1 = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
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
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
            }
        };

        db.BasicPositions.Add(position);
        db.BasicEmployees.AddRange(employee1, employee2);
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

        var returnedEmployees = okResult.Value as Pagination<List<GetAllEmployeeResponse>>;
        Assert.NotNull(returnedEmployees);
        Assert.Equal(2, returnedEmployees.Data.Count);
        Assert.Equal(2, returnedEmployees.Total);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsOkWithEmployee()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        var employee = new EmployeeModel
        {
            Id = testEmployeeId,
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
            }
        };

        db.BasicPositions.Add(position);
        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testEmployeeId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployee = okResult.Value as GetEmployeeByIdResponse;
        Assert.NotNull(returnedEmployee);
        Assert.Equal(testEmployeeId, returnedEmployee.Id);
        Assert.Equal(employee.Person.Id, returnedEmployee.PersonId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithEmployee()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddEmployeeCommand(
            Guid.NewGuid(),
            position.Id,
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

        var returnedEmployee = createdResult.Value as AddEmployeeResponse;
        Assert.NotNull(returnedEmployee);
    }

    [Fact]
    public async Task PatchAsync_WhenEmployeeExists_ReturnsOkWithUpdatedEmployee()
    {

        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        var employee = new EmployeeModel
        {
            Id = testEmployeeId,
            PersonId = Guid.NewGuid(),
            PositionId = position.Id,
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
            }
        };

        db.BasicPositions.Add(position);
        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            employee.Id,
            position.Id,
            faker.Person.FullName + " Updated",
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testEmployeeId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployee = okResult.Value as UpdateEmployeeResponse;
        Assert.NotNull(returnedEmployee);
    }

    [Fact]
    public async Task PatchAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(
            nonExistentId,
            position.Id,
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
    public async Task DeleteAsync_WhenEmployeeExists_ReturnsNoContent()
    {

        var employee = new EmployeeModel
        {
            Id = testEmployeeId,
            PersonId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testEmployeeId, wide, ct);

        Assert.NotNull(result);

        var deletedEmployee = await db.BasicEmployees.FirstOrDefaultAsync(x => x.Id == testEmployeeId && x.Deleted == null, ct);
        Assert.Null(deletedEmployee);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeDoesNotExist_ReturnsNoContent()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
    }

    [Fact]
    public void EmployeeController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(EmployeeController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void EmployeeController_HasRouteAttribute()
    {

        var controllerType = typeof(EmployeeController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void EmployeeController_HasApiControllerAttribute()
    {

        var controllerType = typeof(EmployeeController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }
}
