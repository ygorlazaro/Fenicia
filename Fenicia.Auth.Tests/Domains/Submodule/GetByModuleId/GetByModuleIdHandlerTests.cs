using Bogus;

using Fenicia.Auth.Domains.Submodule.GetByModuleId;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Submodule.GetByModuleId;

[TestFixture]
public class GetByModuleIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new GetByModuleIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private GetByModuleIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenSubmodulesExist_ReturnsSubmodulesForModule()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var submodule1 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Description = this.faker.Commerce.ProductDescription(),
            Route = $"/api/{this.faker.Lorem.Word()}",
            ModuleId = moduleId
        };

        var submodule2 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Description = this.faker.Commerce.ProductDescription(),
            Route = $"/api/{this.faker.Lorem.Word()}",
            ModuleId = moduleId
        };

        this.context.Submodules.AddRange(submodule1, submodule2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(moduleId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2), "Should return 2 submodules");
    }

    [Test]
    public async Task Handle_WhenNoSubmodulesExistForModule_ReturnsEmptyList()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var otherModuleId = Guid.NewGuid();

        var submodule = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            Description = this.faker.Commerce.ProductDescription(),
            Route = $"/api/{this.faker.Lorem.Word()}",
            ModuleId = otherModuleId
        };

        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(moduleId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0), "Should return empty list for module without submodules");
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var moduleId = Guid.NewGuid();

        // Act
        var result = await this.handler.Handle(moduleId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0), "Should return empty list for empty database");
    }

    [Test]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var submoduleId = Guid.NewGuid();
        var submodule = new SubmoduleModel
        {
            Id = submoduleId,
            Name = "Test Submodule",
            Description = "Test Description",
            Route = "/api/test",
            ModuleId = moduleId
        };

        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(moduleId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.GreaterThan(0), "Should have data");
        var response = result[0];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Id, Is.EqualTo(submoduleId), "Id should match");
            Assert.That(response.Name, Is.EqualTo("Test Submodule"), "Name should match");
            Assert.That(response.Description, Is.EqualTo("Test Description"), "Description should match");
            Assert.That(response.ModuleId, Is.EqualTo(moduleId), "ModuleId should match");
            Assert.That(response.Route, Is.EqualTo("/api/test"), "Route should match");
        }
    }

    [Test]
    public async Task Handle_WhenDescriptionIsNull_ReturnsNullInResponse()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var submodule = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Submodule without description",
            Description = null,
            Route = "/api/submodule",
            ModuleId = moduleId
        };

        this.context.Submodules.Add(submodule);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(moduleId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1), "Should return 1 submodule");
        Assert.That(result[0].Description, Is.Null, "Description should be null");
    }

    [Test]
    public async Task Handle_WhenMultipleModulesExist_ReturnsOnlyMatchingSubmodules()
    {
        // Arrange
        var moduleId1 = Guid.NewGuid();
        var moduleId2 = Guid.NewGuid();

        var submodule1 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Module 1 - Submodule 1",
            Description = "Desc 1",
            Route = "/api/m1/s1",
            ModuleId = moduleId1
        };

        var submodule2 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Module 1 - Submodule 2",
            Description = "Desc 2",
            Route = "/api/m1/s2",
            ModuleId = moduleId1
        };

        var submodule3 = new SubmoduleModel
        {
            Id = Guid.NewGuid(),
            Name = "Module 2 - Submodule 1",
            Description = "Desc 3",
            Route = "/api/m2/s1",
            ModuleId = moduleId2
        };

        this.context.Submodules.AddRange(submodule1, submodule2, submodule3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(moduleId1, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(2), "Should return only submodules for module1");
            Assert.That(result.All(sm => sm.ModuleId == moduleId1), Is.True, "All submodules should belong to module1");
        }
    }

    [Test]
    public async Task Handle_VerifiesSubmodulesAreFilteredCorrectly()
    {
        // Arrange
        var moduleId1 = Guid.NewGuid();
        var moduleId2 = Guid.NewGuid();

        for (var i = 0; i < 5; i++)
        {
            this.context.Submodules.Add(new SubmoduleModel
            {
                Id = Guid.NewGuid(),
                Name = $"Submodule {i}",
                Description = $"Description {i}",
                Route = $"/api/m1/s{i}",
                ModuleId = moduleId1
            });
        }

        for (var i = 0; i < 3; i++)
        {
            this.context.Submodules.Add(new SubmoduleModel
            {
                Id = Guid.NewGuid(),
                Name = $"Submodule {i}",
                Description = $"Description {i}",
                Route = $"/api/m2/s{i}",
                ModuleId = moduleId2
            });
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result1 = await this.handler.Handle(moduleId1, CancellationToken.None);
        var result2 = await this.handler.Handle(moduleId2, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Has.Count.EqualTo(5), "Module1 should have 5 submodules");
            Assert.That(result2, Has.Count.EqualTo(3), "Module2 should have 3 submodules");
        }
    }
}