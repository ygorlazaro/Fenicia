using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class SupplierControllerTests : IDisposable
{
    private readonly SupplierController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testSupplierId;

    public SupplierControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testSupplierId = Guid.NewGuid();
        var getAllSupplierHandler = new GetAllSupplierHandler(db);
        var getSupplierByIdHandler = new GetSupplierByIdHandler(db);
        var addSupplierHandler = new AddSupplierHandler(db);
        var updateSupplierHandler = new UpdateSupplierHandler(db);
        var deleteSupplierHandler = new DeleteSupplierHandler(db);
        var getSupplierPerformanceHandler = new GetSupplierPerformanceHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new SupplierController(getAllSupplierHandler, getSupplierByIdHandler, addSupplierHandler, updateSupplierHandler, deleteSupplierHandler, getSupplierPerformanceHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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
    public async Task GetAsync_WhenNoSuppliersExist_ReturnsOkWithEmptyList()
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

        var returnedSuppliers = okResult.Value as Pagination<List<GetAllSupplierResponse>>;
        Assert.NotNull(returnedSuppliers);
        Assert.Empty(returnedSuppliers.Data);
        Assert.Equal(0, returnedSuppliers.Total);
    }

    [Fact]
    public async Task GetAsync_WhenSuppliersExist_ReturnsOkWithSuppliers()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var supplier1 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            Cnpj = faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("###"),
                Complement = "Suite 100",
                Neighborhood = faker.Address.CityPrefix(),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City()
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            Cnpj = faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("###"),
                Complement = "Suite 200",
                Neighborhood = faker.Address.CityPrefix(),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City()
            }
        };

        db.BasicSuppliers.AddRange(supplier1, supplier2);
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

        var returnedSuppliers = okResult.Value as Pagination<List<GetAllSupplierResponse>>;
        Assert.NotNull(returnedSuppliers);
        Assert.Equal(2, returnedSuppliers.Data.Count);
        Assert.Equal(2, returnedSuppliers.Total);
        Assert.Equal(supplier1.Person.Name, returnedSuppliers.Data[0].Name);
        Assert.Equal(supplier1.Person.Email, returnedSuppliers.Data[0].Email);
        Assert.Equal(supplier2.Person.Name, returnedSuppliers.Data[1].Name);
        Assert.Equal(supplier2.Person.Email, returnedSuppliers.Data[1].Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsOkWithSupplier()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var supplier = new SupplierModel
        {
            Id = testSupplierId,
            Cnpj = faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####"),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("###"),
                Complement = "Suite 100",
                Neighborhood = faker.Address.CityPrefix(),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City()
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testSupplierId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSupplier = okResult.Value as GetSupplierByIdResponse;
        Assert.NotNull(returnedSupplier);
        Assert.Equal(testSupplierId, returnedSupplier.Id);
        Assert.Equal(supplier.Person.Id, returnedSupplier.PersonId);
        Assert.Equal(supplier.Person.Name, returnedSupplier.Name);
        Assert.Equal(supplier.Person.Email, returnedSupplier.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNotFound()
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

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithSupplier()
    {
        // Arrange
        var command = new AddSupplierCommand(Guid.NewGuid(), faker.Company.CompanyName(), faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), "Suite 100", faker.Address.CityPrefix(), faker.Random.Replace("####"), Guid.NewGuid(), faker.Address.StreetName(), faker.Address.ZipCode(), faker.Random.Replace("(##) #####-####"), faker.Company.Cnpj());

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

        var returnedSupplier = createdResult.Value as AddSupplierResponse;
        Assert.NotNull(returnedSupplier);
        Assert.Equal(command.Cnpj, returnedSupplier.Cnpj);
    }

    [Fact]
    public async Task PatchAsync_WhenSupplierExists_ReturnsOkWithUpdatedSupplier()
    {
        // Arrange
        var supplier = new SupplierModel
        {
            Id = testSupplierId,
            Cnpj = faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####")
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(testSupplierId, faker.Company.CompanyName() + " Updated", faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.Empty, null, null, null, faker.Company.Cnpj());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testSupplierId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSupplier = okResult.Value as UpdateSupplierResponse;
        Assert.NotNull(returnedSupplier);
    }

    [Fact]
    public async Task PatchAsync_WhenSupplierDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateSupplierCommand(nonExistentId, faker.Company.CompanyName(), faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.Empty, null, null, null, faker.Company.Cnpj());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierExists_ReturnsNoContent()
    {
        // Arrange
        var supplier = new SupplierModel
        {
            Id = testSupplierId,
            Cnpj = faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName(),
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testSupplierId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify supplier was deleted
        var deletedSupplier = await db.BasicSuppliers.FirstOrDefaultAsync(x => x.Id == testSupplierId && x.Deleted == null, ct);
        Assert.Null(deletedSupplier);
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierDoesNotExist_ReturnsNoContent()
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

    [Fact]
    public void SupplierController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(SupplierController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void SupplierController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(SupplierController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void SupplierController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(SupplierController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}
