namespace Fenicia.Auth.Tests.Services;

using System.Net;

using AutoMapper;

using Bogus;

using Common;
using Common.Enums;

using Domains.Module.Data;
using Domains.Module.Logic;

using Microsoft.Extensions.Logging;

using Moq;

public class ModuleServiceTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Faker _faker;
    private Mock<ILogger<ModuleService>> _loggerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IModuleRepository> _moduleRepositoryMock;
    private ModuleService _sut;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ModuleService>>();
        _moduleRepositoryMock = new Mock<IModuleRepository>();
        _sut = new ModuleService(_mapperMock.Object, _loggerMock.Object, _moduleRepositoryMock.Object);
        _faker = new Faker();
    }

    [Test]
    public async Task GetAllOrderedAsync_ReturnsModules()
    {
        // Arrange
        var modules = new List<ModuleModel>
                      {
                          new() { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName() },
                          new() { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName() }
                      };
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        _moduleRepositoryMock.Setup(x => x.GetAllOrderedAsync(_cancellationToken, 1, 10)).ReturnsAsync(modules);

        _mapperMock.Setup(x => x.Map<List<ModuleResponse>>(modules)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetAllOrderedAsync(_cancellationToken);

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
        var moduleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var modules = moduleIds.Select(id => new ModuleModel { Id = id, Name = _faker.Commerce.ProductName() }).ToList();
        var expectedResponse = modules.Select(m => new ModuleResponse { Id = m.Id, Name = m.Name }).ToList();

        _moduleRepositoryMock.Setup(x => x.GetManyOrdersAsync(moduleIds, _cancellationToken)).ReturnsAsync(modules);

        _mapperMock.Setup(x => x.Map<List<ModuleResponse>>(modules)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetModulesToOrderAsync(moduleIds, _cancellationToken);

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
                         Name = _faker.Commerce.ProductName(),
                         Type = moduleType
                     };
        var expectedResponse = new ModuleResponse
                               {
                                   Id = module.Id,
                                   Name = module.Name,
                                   Type = moduleType
                               };

        _moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, _cancellationToken)).ReturnsAsync(module);

        _mapperMock.Setup(x => x.Map<ModuleResponse>(module)).Returns(expectedResponse);

        // Act
        var result = await _sut.GetModuleByTypeAsync(moduleType, _cancellationToken);

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

        _moduleRepositoryMock.Setup(x => x.GetModuleByTypeAsync(moduleType, _cancellationToken)).ReturnsAsync((ModuleModel)null!);

        // Act
        var result = await _sut.GetModuleByTypeAsync(moduleType, _cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Message, Is.EqualTo(TextConstants.ItemNotFound));
        });
    }

    [Test]
    public async Task CountAsync_ReturnsCount()
    {
        // Arrange
        var expectedCount = _faker.Random.Int(min: 1, max: 100);

        _moduleRepositoryMock.Setup(x => x.CountAsync(_cancellationToken)).ReturnsAsync(expectedCount);

        // Act
        var result = await _sut.CountAsync(_cancellationToken);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedCount));
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}
