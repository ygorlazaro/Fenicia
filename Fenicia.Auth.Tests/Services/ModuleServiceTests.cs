using Bogus;

using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;

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
        this.moduleRepositoryMock = new Mock<IModuleRepository>();
        this.sut = new ModuleService(this.moduleRepositoryMock.Object);
        this.faker = new Faker();
    }

    [Test]
    public async Task GetAllOrderedAsyncReturnsModules()
    {
        var modules = new List<ModuleModel>
        {
            new() { Id = Guid.NewGuid(), Name = this.faker.Commerce.ProductName() },
            new() { Id = Guid.NewGuid(), Name = this.faker.Commerce.ProductName() }
        };
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        this.moduleRepositoryMock.Setup(x => x.GetAllAsync(this.cancellationToken, 1, 10)).ReturnsAsync(modules);

        var result = await this.sut.GetAllOrderedAsync(this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModulesToOrderAsyncReturnsRequestedModules()
    {
        var moduleIDs = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var modules = moduleIDs.Select(id => new ModuleModel { Id = id, Name = this.faker.Commerce.ProductName() })
            .ToList();
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        this.moduleRepositoryMock.Setup(x => x.GetManyOrdersAsync(moduleIDs, this.cancellationToken))
            .ReturnsAsync(modules);

        var result = await this.sut.GetModulesToOrderAsync(moduleIDs, this.cancellationToken);

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
            Name = this.faker.Commerce.ProductName(),
            Type = moduleType
        };
        var expectedResponse = new ModuleResponse
        {
            Id = module.Id,
            Name = module.Name,
            Type = moduleType
        };

        this.moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, this.cancellationToken))
            .ReturnsAsync(module);

        var result = await this.sut.GetModuleByTypeAsync(moduleType, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expectedResponse));
        }
    }

    [Test]
    public async Task GetModuleByTypeAsyncWhenModuleDoesNotExistReturnsNull()
    {
        const ModuleType moduleType = ModuleType.Ecommerce;

        this.moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, this.cancellationToken))
            .ReturnsAsync((ModuleModel?)null);

        var result = await this.sut.GetModuleByTypeAsync(moduleType, this.cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CountAsyncReturnsCount()
    {
        var expectedCount = this.faker.Random.Int(1, 100);

        this.moduleRepositoryMock.Setup(x => x.CountAsync(this.cancellationToken)).ReturnsAsync(expectedCount);

        var result = await this.sut.CountAsync(this.cancellationToken);

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
            new() { Id = Guid.NewGuid(), Name = "A", Price = 1, Type = ModuleType.Accounting },
            new() { Id = Guid.NewGuid(), Name = "B", Price = 2, Type = ModuleType.Basic }
        };

        this.moduleRepositoryMock
            .Setup(x => x.LoadModulesAtDatabaseAsync(It.IsAny<List<ModuleModel>>(), this.cancellationToken))
            .ReturnsAsync(modules);

        var result = await this.sut.LoadModulesAtDatabaseAsync(this.cancellationToken);

        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(modules.Select(m => m.Id)));
    }

    [Test]
    public async Task GetUserModulesAsync_ReturnsConvertedModules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleModel>
        {
            new() { Id = Guid.NewGuid(), Name = "U1", Price = 1, Type = ModuleType.Basic }
        };

        this.moduleRepositoryMock.Setup(x => x.GetUserModulesAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(modules);

        var result = await this.sut.GetUserModulesAsync(userId, companyId, this.cancellationToken);

        Assert.That(result.Select(r => r.Id), Is.EquivalentTo(modules.Select(m => m.Id)));
    }

    [Test]
    public async Task GetModuleAndSubmoduleAsync_ReturnsConvertedModulesWithSubmodules()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module = new ModuleModel { Id = Guid.NewGuid(), Name = "WithSub", Price = 1, Type = ModuleType.Basic };
        var sub = new SubmoduleModel { Id = Guid.NewGuid(), Name = "s", Route = "/s", ModuleId = module.Id };
        module.Submodules = [sub];
        var modules = new List<ModuleModel> { module };

        this.moduleRepositoryMock.Setup(x => x.GetModuleAndSubmoduleAsync(userId, companyId, this.cancellationToken))
            .ReturnsAsync(modules);

        var result = await this.sut.GetModuleAndSubmoduleAsync(userId, companyId, this.cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.First().Id, Is.EqualTo(module.Id));
            Assert.That(result.First().Submodules, Is.Not.Null);
        }

        Assert.That(result.First().Submodules.First().Id, Is.EqualTo(sub.Id));
    }
}