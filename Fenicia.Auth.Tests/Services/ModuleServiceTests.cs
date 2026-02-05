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
        var modules = new List<ModuleModel>
                      {
                          new () { Id = Guid.NewGuid(), Name = faker.Commerce.ProductName() },
                          new () { Id = Guid.NewGuid(), Name = faker.Commerce.ProductName() }
                      };
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        moduleRepositoryMock.Setup(x => x.GetAllAsync(cancellationToken, 1, 10)).ReturnsAsync(modules);

        var result = await sut.GetAllOrderedAsync(cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModulesToOrderAsyncReturnsRequestedModules()
    {
        var moduleIDs = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var modules = moduleIDs.Select(id => new ModuleModel { Id = id, Name = faker.Commerce.ProductName() }).ToList();
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        moduleRepositoryMock.Setup(x => x.GetManyOrdersAsync(moduleIDs, cancellationToken)).ReturnsAsync(modules);

        var result = await sut.GetModulesToOrderAsync(moduleIDs, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModuleByTypeAsyncWhenModuleExistsReturnsModule()
    {
        const ModuleType moduleType = ModuleType.Ecommerce;
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

        var result = await sut.GetModuleByTypeAsync(moduleType, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModuleByTypeAsyncWhenModuleDoesNotExistReturnsNull()
    {
        const ModuleType moduleType = ModuleType.Ecommerce;

        moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, cancellationToken)).ReturnsAsync((ModuleModel?)null);

        var result = await sut.GetModuleByTypeAsync(moduleType, cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CountAsyncReturnsCount()
    {
        var expectedCount = faker.Random.Int(min: 1, max: 100);

        moduleRepositoryMock.Setup(x => x.CountAsync(cancellationToken)).ReturnsAsync(expectedCount);

        var result = await sut.CountAsync(cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expectedCount));
        }
    }

    [Test]
    public async Task LoadModulesAtDatabaseAsync_DelegatesAndReturnsConverted()
    {
        var modules = new List<ModuleModel>
        {
            new () { Id = Guid.NewGuid(), Name = "A", Amount = 1, Type = ModuleType.Accounting },
            new () { Id = Guid.NewGuid(), Name = "B", Amount = 2, Type = ModuleType.Basic }
        };

        moduleRepositoryMock.Setup(x => x.LoadModulesAtDatabaseAsync(It.IsAny<List<ModuleModel>>(), cancellationToken)).ReturnsAsync(modules);

        var result = await sut.LoadModulesAtDatabaseAsync(cancellationToken);

        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(modules.Select(m => m.Id)));
    }

    [Test]
    public async Task GetUserModulesAsync_ReturnsConvertedModules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleModel>
        {
            new () { Id = Guid.NewGuid(), Name = "U1", Amount = 1, Type = ModuleType.Basic }
        };

        moduleRepositoryMock.Setup(x => x.GetUserModulesAsync(userId, companyId, cancellationToken)).ReturnsAsync(modules);

        var result = await sut.GetUserModulesAsync(userId, companyId, cancellationToken);

        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(modules.Select(m => m.Id)));
    }

    [Test]
    public async Task GetModuleAndSubmoduleAsync_ReturnsConvertedModulesWithSubmodules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module = new ModuleModel { Id = Guid.NewGuid(), Name = "WithSub", Amount = 1, Type = ModuleType.Basic };
        var sub = new SubmoduleModel { Id = Guid.NewGuid(), Name = "s", Route = "/s", ModuleId = module.Id };
        module.Submodules = [sub];
        var modules = new List<ModuleModel> { module };

        moduleRepositoryMock.Setup(x => x.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken)).ReturnsAsync(modules);

        var result = await sut.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.First().Id, Is.EqualTo(module.Id));
            Assert.That(result.First().Submodules, Is.Not.Null);
        }

        Assert.That(result.First().Submodules.First().Id, Is.EqualTo(sub.Id));
    }
}
