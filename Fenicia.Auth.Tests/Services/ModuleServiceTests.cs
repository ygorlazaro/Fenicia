namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Bogus;

using Common.Database.Responses;
using Common.Enums;

using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.Extensions.Logging;

using Moq;

public class ModuleServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private Mock<ILogger<ModuleService>> loggerMock;
    private Mock<IModuleRepository> moduleRepositoryMock;
    private ModuleService sut;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<ModuleService>>();
        this.moduleRepositoryMock = new Mock<IModuleRepository>();
        this.sut = new ModuleService(this.loggerMock.Object, this.moduleRepositoryMock.Object);
        this.faker = new Faker();
    }

    [Test]
    public async Task GetAllOrderedAsync_ReturnsModules()
    {
        // Arrange
        var modules = new List<ModuleModel>
                      {
                          new () { Id = Guid.NewGuid(), Name = this.faker.Commerce.ProductName() },
                          new () { Id = Guid.NewGuid(), Name = this.faker.Commerce.ProductName() }
                      };
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        this.moduleRepositoryMock.Setup(x => x.GetAllOrderedAsync(this.cancellationToken, 1, 10)).ReturnsAsync(modules);

        // Act
        var result = await this.sut.GetAllOrderedAsync(this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetModulesToOrderAsync_ReturnsRequestedModules()
    {
        // Arrange
        var moduleIDs = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var modules = moduleIDs.Select(id => new ModuleModel { Id = id, Name = this.faker.Commerce.ProductName() }).ToList();
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        this.moduleRepositoryMock.Setup(x => x.GetManyOrdersAsync(moduleIDs, this.cancellationToken)).ReturnsAsync(modules);

        // Act
        var result = await this.sut.GetModulesToOrderAsync(moduleIDs, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetModuleByTypeAsync_WhenModuleExists_ReturnsModule()
    {
        // Arrange
        var moduleType = ModuleType.Ecommerce;
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

        this.moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, this.cancellationToken)).ReturnsAsync(module);

        // Act
        var result = await this.sut.GetModuleByTypeAsync(moduleType, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedResponse));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public async Task GetModuleByTypeAsync_WhenModuleDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const ModuleType moduleType = ModuleType.Ecommerce;

        this.moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, this.cancellationToken)).ReturnsAsync((ModuleModel)null!);

        // Act
        var result = await this.sut.GetModuleByTypeAsync(moduleType, this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task CountAsync_ReturnsCount()
    {
        // Arrange
        var expectedCount = this.faker.Random.Int(min: 1, max: 100);

        this.moduleRepositoryMock.Setup(x => x.CountAsync(this.cancellationToken)).ReturnsAsync(expectedCount);

        // Act
        var result = await this.sut.CountAsync(this.cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}
