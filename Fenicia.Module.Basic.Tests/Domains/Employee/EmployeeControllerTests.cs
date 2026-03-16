using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

/// <summary>
///     Unit tests for the EmployeeController.
///     Tests HTTP endpoints behavior including CRUD operations, pagination, and request/response handling.
/// </summary>
public class EmployeeControllerTests : IDisposable
{
    private readonly EmployeeController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
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
        mockHttpContext = new Mock<HttpContext>();

        controller = new EmployeeController(getAllEmployeeHandler, getEmployeeByIdHandler, addEmployeeHandler, updateEmployeeHandler, deleteEmployeeHandler, getEmployeePerformanceHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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

    /// <summary>
    ///     Tests that when no employees exist, the endpoint returns an empty paginated response.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenNoEmployeesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as Pagination<List<GetAllEmployeeResponse>>;
        Assert.NotNull(returnedEmployees);
        Assert.Empty(returnedEmployees.Data);
        Assert.Equal(0, returnedEmployees.Total);
    }

    /// <summary>
    ///     Tests that when employees exist, the endpoint returns them in a paginated response.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenEmployeesExist_ReturnsOkWithEmployees()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = faker.Address.State(), Uf = faker.Address.StateAbbr() };
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
                StateId = state.Id,
                State = state
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
                StateId = state.Id,
                State = state
            }
        };

        db.BasicPositions.Add(position);
        db.AuthStates.Add(state);
        db.BasicEmployees.AddRange(employee1, employee2);
        await db.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployees = okResult.Value as Pagination<List<GetAllEmployeeResponse>>;
        Assert.NotNull(returnedEmployees);
        Assert.Equal(2, returnedEmployees.Data.Count);
        Assert.Equal(2, returnedEmployees.Total);
    }

    /// <summary>
    ///     Tests that when an employee exists, the endpoint returns the employee details.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsOkWithEmployee()
    {
        // Arrange
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
                PhoneNumber = faker.Random.Replace("(##) #####-####")
            }
        };

        db.BasicPositions.Add(position);
        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testEmployeeId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployee = okResult.Value as GetEmployeeByIdResponse;
        Assert.NotNull(returnedEmployee);
        Assert.Equal(testEmployeeId, returnedEmployee.Id);
        Assert.Equal(employee.Person.Id, returnedEmployee.PersonId);
    }

    /// <summary>
    ///     Tests that when an employee does not exist, the endpoint returns NotFound.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    /// <summary>
    ///     Tests that creating an employee with valid data returns Created result with employee data.
    /// </summary>
    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithEmployee()
    {
        // Arrange
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddEmployeeCommand(Guid.NewGuid(), position.Id, faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), "Apt 101", faker.Address.CityPrefix(), faker.Random.Replace("####"), Guid.NewGuid(), faker.Address.StreetName(), faker.Address.ZipCode(), faker.Random.Replace("(##) #####-####"));

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedEmployee = createdResult.Value as AddEmployeeResponse;
        Assert.NotNull(returnedEmployee);
    }

    /// <summary>
    ///     Tests that updating an existing employee returns Ok with the updated employee.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenEmployeeExists_ReturnsOkWithUpdatedEmployee()
    {
        // Arrange
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
                PhoneNumber = faker.Random.Replace("(##) #####-####")
            }
        };

        db.BasicPositions.Add(position);
        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(employee.Id, position.Id, faker.Person.FullName + " Updated", faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.Empty, null, null, null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testEmployeeId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedEmployee = okResult.Value as UpdateEmployeeResponse;
        Assert.NotNull(returnedEmployee);
    }

    /// <summary>
    ///     Tests that updating a non-existent employee returns NotFound.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Department()
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(nonExistentId, position.Id, faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.Empty, null, null, null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    /// <summary>
    ///     Tests that deleting an existing employee returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_ReturnsNoContent()
    {
        // Arrange
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
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testEmployeeId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify employee was deleted
        var deletedEmployee = await db.BasicEmployees.FirstOrDefaultAsync(x => x.Id == testEmployeeId && x.Deleted == null, ct);
        Assert.Null(deletedEmployee);
    }

    /// <summary>
    ///     Tests that deleting a non-existent employee returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WhenEmployeeDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that the EmployeeController has the AuthorizeAttribute applied.
    /// </summary>
    [Fact]
    public void EmployeeController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(EmployeeController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    /// <summary>
    ///     Tests that the EmployeeController has the RouteAttribute with correct template.
    /// </summary>
    [Fact]
    public void EmployeeController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(EmployeeController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    /// <summary>
    ///     Tests that the EmployeeController has the ApiControllerAttribute applied.
    /// </summary>
    [Fact]
    public void EmployeeController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(EmployeeController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}
