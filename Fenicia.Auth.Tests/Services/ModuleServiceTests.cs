using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class ModuleServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private Mock<IModuleRepository> moduleRepositoryMock;
    private ModuleService sut;

    [SetUp]
    public void Setup()
    {
        moduleRepositoryMock = new Mock<IModuleRepository>();
        sut = new ModuleService(moduleRepositoryMock.Object);
        faker = new Faker();
    }

    [Test]
    public async Task GetAllOrderedAsyncReturnsModules()
    {
        // Arrange
        var modules = new List<ModuleModel>
                      {
                          new () { Id = Guid.NewGuid(), Name = faker.Commerce.ProductName() },
                          new () { Id = Guid.NewGuid(), Name = faker.Commerce.ProductName() }
                      };
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        moduleRepositoryMock.Setup(x => x.GetAllOrderedAsync(cancellationToken, 1, 10)).ReturnsAsync(modules);

        // Act
        var result = await sut.GetAllOrderedAsync(cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModulesToOrderAsyncReturnsRequestedModules()
    {
        // Arrange
        var moduleIDs = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var modules = moduleIDs.Select(id => new ModuleModel { Id = id, Name = faker.Commerce.ProductName() }).ToList();
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        moduleRepositoryMock.Setup(x => x.GetManyOrdersAsync(moduleIDs, cancellationToken)).ReturnsAsync(modules);

        // Act
        var result = await sut.GetModulesToOrderAsync(moduleIDs, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModuleByTypeAsyncWhenModuleExistsReturnsModule()
    {
        // Arrange
        var moduleType = ModuleType.Ecommerce;
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Type = moduleType
        };
        var expectedResponse = new ModuleResponse
        {
            Id = module.Id,
            Name = module.Name,
            Type = moduleType
        };

        moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, cancellationToken)).ReturnsAsync(module);

        // Act
        var result = await sut.GetModuleByTypeAsync(moduleType, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task CountAsyncReturnsCount()
    {
        // Arrange
        var expectedCount = faker.Random.Int(min: 1, max: 100);

        moduleRepositoryMock.Setup(x => x.CountAsync(cancellationToken)).ReturnsAsync(expectedCount);

        // Act
        var result = await sut.CountAsync(cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
        }
    }
}
