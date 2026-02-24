using System.Security.Claims;

using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.Add;
using Fenicia.Module.Basic.Domains.Supplier.Delete;
using Fenicia.Module.Basic.Domains.Supplier.GetAll;
using Fenicia.Module.Basic.Domains.Supplier.GetById;
using Fenicia.Module.Basic.Domains.Supplier.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

[TestFixture]
public class SupplierControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testSupplierId = Guid.NewGuid();
        this.getAllSupplierHandler = new GetAllSupplierHandler(this.context);
        this.getSupplierByIdHandler = new GetSupplierByIdHandler(this.context);
        this.addSupplierHandler = new AddSupplierHandler(this.context);
        this.updateSupplierHandler = new UpdateSupplierHandler(this.context);
        this.deleteSupplierHandler = new DeleteSupplierHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new SupplierController(
            this.getAllSupplierHandler,
            this.getSupplierByIdHandler,
            this.addSupplierHandler,
            this.updateSupplierHandler,
            this.deleteSupplierHandler)
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

    private SupplierController controller = null!;
    private BasicContext context = null!;
    private GetAllSupplierHandler getAllSupplierHandler = null!;
    private GetSupplierByIdHandler getSupplierByIdHandler = null!;
    private AddSupplierHandler addSupplierHandler = null!;
    private UpdateSupplierHandler updateSupplierHandler = null!;
    private DeleteSupplierHandler deleteSupplierHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testSupplierId;
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
    public async Task GetAsync_WhenNoSuppliersExist_ReturnsOkWithEmptyList()
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

        var returnedSuppliers = okResult.Value as List<GetAllSupplierResponse>;
        Assert.That(returnedSuppliers, Is.Not.Null);
        Assert.That(returnedSuppliers, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenSuppliersExist_ReturnsOkWithSuppliers()
    {
        // Arrange
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
                StateId = Guid.NewGuid(),
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Suppliers.AddRange(supplier1, supplier2);
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

        var returnedSuppliers = okResult.Value as List<GetAllSupplierResponse>;
        Assert.That(returnedSuppliers, Is.Not.Null);
        Assert.That(returnedSuppliers, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsOkWithSupplier()
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
                PhoneNumber = this.faker.Random.Replace("(##) #####-####"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("###"),
                Complement = "Suite 100",
                Neighborhood = this.faker.Address.CityPrefix(),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Suppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testSupplierId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSupplier = okResult.Value as GetSupplierByIdResponse;
        Assert.That(returnedSupplier, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedSupplier.Id, Is.EqualTo(this.testSupplierId));
            Assert.That(returnedSupplier.Cnpj, Is.EqualTo(supplier.Cnpj));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNotFound()
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedSupplier = createdResult.Value as AddSupplierResponse;
        Assert.That(returnedSupplier, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedSupplier.Cnpj, Is.EqualTo(command.Cnpj));
        }
    }

    [Test]
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

        this.context.Suppliers.Add(supplier);
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testSupplierId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSupplier = okResult.Value as UpdateSupplierResponse;
        Assert.That(returnedSupplier, Is.Not.Null);
    }

    [Test]
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
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

        this.context.Suppliers.Add(supplier);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testSupplierId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify supplier was deleted
        var deletedSupplier = await this.context.Suppliers.FirstOrDefaultAsync(x => x.Id == this.testSupplierId, cancellationToken);
        Assert.That(deletedSupplier, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenSupplierDoesNotExist_ReturnsNoContent()
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
    public void SupplierController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(SupplierController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "SupplierController should have Authorize attribute");
    }

    [Test]
    public void SupplierController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(SupplierController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "SupplierController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void SupplierController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(SupplierController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "SupplierController should have ApiController attribute");
    }
}
