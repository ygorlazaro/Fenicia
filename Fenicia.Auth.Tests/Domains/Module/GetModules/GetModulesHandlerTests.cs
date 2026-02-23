using Bogus;

using Fenicia.Auth.Domains.Module.GetModules;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Module.GetModules;

[TestFixture]
public class GetModulesHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new GetModulesHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private GetModulesHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenModulesExist_ReturnsPaginatedModules()
    {
        // Arrange
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m
        };

        this.context.Modules.AddRange(module1, module2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(2), "Should return 2 modules");
            Assert.That(result.Total, Is.EqualTo(2), "Total should be 2");
            Assert.That(result.Page, Is.EqualTo(1), "Page should be 1");
            Assert.That(result.PerPage, Is.EqualTo(10), "PerPage should be 10");
        }
    }

    [Test]
    public async Task Handle_WhenModulesExist_ExcludesErpAndAuthTypes()
    {
        // Arrange
        var erpModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Erp,
            Price = 100.0m
        };

        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m
        };

        var basicModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.context.Modules.AddRange(erpModule, authModule, basicModule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(1), "Should exclude ERP and Auth modules");
            Assert.That(result.Data[0].Name, Is.EqualTo(basicModule.Name), "Should return only Basic module");
            Assert.That(result.Total, Is.EqualTo(1), "Total should be 1");
        }
    }

    [Test]
    public async Task Handle_WhenPaginationIsApplied_ReturnsCorrectPage()
    {
        // Arrange
        var modules = new List<ModuleModel>();
        for (var i = 0; i < 25; i++)
        {
            modules.Add(new ModuleModel
            {
                Id = Guid.NewGuid(),
                Name = $"Module {this.faker.Commerce.ProductName()} {i}",
                Type = (ModuleType)(i % 10 + 1),
                Price = 10.0m
            });
        }

        this.context.Modules.AddRange(modules);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(2, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(10), "Should return 10 modules for page 2");
            Assert.That(result.Total, Is.EqualTo(25), "Total should be 25");
            Assert.That(result.Page, Is.EqualTo(2), "Page should be 2");
            Assert.That(result.PerPage, Is.EqualTo(10), "PerPage should be 10");
            Assert.That(result.Pages, Is.EqualTo(3), "Should have 3 pages total");
        }
    }

    [Test]
    public async Task Handle_WhenNoModulesExist_ReturnsEmptyPagination()
    {
        // Arrange
        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Empty, "Should return empty list");
            Assert.That(result.Total, Is.EqualTo(0), "Total should be 0");
        }
    }

    [Test]
    public async Task Handle_WhenPageExceedsTotalPages_ReturnsEmptyData()
    {
        // Arrange
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = "Basic Module",
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.context.Modules.Add(module);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(10, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Empty, "Should return empty data for page beyond total");
            Assert.That(result.Total, Is.EqualTo(1), "Total should still be 1");
        }
    }

    [Test]
    public async Task Handle_ResultsAreOrderedByType()
    {
        // Arrange
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        var module3 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Hr,
            Price = 30.0m
        };

        this.context.Modules.AddRange(module1, module2, module3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Has.Count.EqualTo(3), "Should return 3 modules");
            Assert.That(result.Data[0].Type, Is.EqualTo(ModuleType.Basic), "First should be Basic");
            Assert.That(result.Data[1].Type, Is.EqualTo(ModuleType.SocialNetwork), "Second should be SocialNetwork");
            Assert.That(result.Data[2].Type, Is.EqualTo(ModuleType.Hr), "Third should be Hr");
        }
    }

    [Test]
    public async Task Handle_WithDefaultRequest_ReturnsFirstPage()
    {
        // Arrange
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.context.Modules.Add(module);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest();

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Page, Is.EqualTo(1), "Default page should be 1");
            Assert.That(result.PerPage, Is.EqualTo(20), "Default PerPage should be 20");
        }
    }

    [Test]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var module = new ModuleModel
        {
            Id = moduleId,
            Name = this.faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m
        };

        this.context.Modules.Add(module);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var request = new GetModulesRequest(1, 10);

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data.Count, Is.GreaterThan(0), "Should have data");
        var moduleResponse = result.Data[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(moduleResponse.Id, Is.EqualTo(moduleId), "Id should match");
            Assert.That(moduleResponse.Name, Is.EqualTo(module.Name), "Name should match");
            Assert.That(moduleResponse.Type, Is.EqualTo(ModuleType.Basic), "Type should match");
        }
    }
}