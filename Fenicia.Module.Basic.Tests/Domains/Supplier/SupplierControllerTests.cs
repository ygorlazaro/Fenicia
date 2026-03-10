using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.Add;
using Fenicia.Module.Basic.Domains.Supplier.Delete;
using Fenicia.Module.Basic.Domains.Supplier.GetAll;
using Fenicia.Module.Basic.Domains.Supplier.GetById;
using Fenicia.Module.Basic.Domains.Supplier.GetSupplierPerformance;
using Fenicia.Module.Basic.Domains.Supplier.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class SupplierControllerTests : IDisposable
{
    public SupplierControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testSupplierId = Guid.NewGuid();
        var getAllSupplierHandler = new GetAllSupplierHandler(this.context);
        var getSupplierByIdHandler = new GetSupplierByIdHandler(this.context);
        var addSupplierHandler = new AddSupplierHandler(this.context);
        var updateSupplierHandler = new UpdateSupplierHandler(this.context);
        var deleteSupplierHandler = new DeleteSupplierHandler(this.context);
        var getSupplierPerformanceHandler = new GetSupplierPerformanceHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new SupplierController(
            getAllSupplierHandler,
            getSupplierByIdHandler,
            addSupplierHandler,
            updateSupplierHandler,
            deleteSupplierHandler,
            getSupplierPerformanceHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    private readonly SupplierController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testSupplierId;
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
    public async Task GetAsync_WhenNoSuppliersExist_ReturnsOkWithEmptyList()
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
        this.context.AuthStates.Add(state);

        var supplier1 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            Cnpj = this.faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Suite 100",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        var supplier2 = new SupplierModel
        {
            Id = Guid.NewGuid(),
            Cnpj = this.faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Suite 200",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        this.context.BasicSuppliers.AddRange(supplier1, supplier2);
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
        this.context.AuthStates.Add(state);

        var supplier = new SupplierModel
        {
            Id = this.testSupplierId,
            Cnpj = this.faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Suite 100",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City()
            }
        };

        this.context.BasicSuppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testSupplierId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSupplier = okResult.Value as GetSupplierByIdResponse;
        Assert.NotNull(returnedSupplier);
        Assert.Equal(this.testSupplierId, returnedSupplier.Id);
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
        var result = await this.controller.GetByIdAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithSupplier()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(),
            this.faker.Company.CompanyName(),
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Suite 100",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Company.Cnpj());

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
            Id = this.testSupplierId,
            Cnpj = this.faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                PhoneNumber = this.faker.Random.Replace("(##) #####-####")
            }
        };

        this.context.BasicSuppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            this.testSupplierId,
            this.faker.Company.CompanyName() + " Updated",
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.Empty,
            null,
            null,
            null,
            this.faker.Company.Cnpj());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testSupplierId, wide, ct);

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
        var command = new UpdateSupplierCommand(
            nonExistentId,
            this.faker.Company.CompanyName(),
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.Empty,
            null,
            null,
            null,
            this.faker.Company.Cnpj());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

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
            Id = this.testSupplierId,
            Cnpj = this.faker.Company.Cnpj(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Company.CompanyName(),
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        this.context.BasicSuppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testSupplierId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify supplier was deleted
        var deletedSupplier = await this.context.BasicSuppliers.FirstOrDefaultAsync(x => x.Id == this.testSupplierId && x.Deleted == null, ct);
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
        var result = await this.controller.DeleteAsync(nonExistentId, wide, ct);

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
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

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
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
