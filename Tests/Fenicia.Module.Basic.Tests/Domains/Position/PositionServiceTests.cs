using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionServiceTests : IDisposable
{
    private readonly DbContextOptions<DefaultContext> _dbOptions;
    private readonly Faker _faker;
    private readonly Mock<IPositionRepository> _mockRepository;
    private readonly PositionService _service;

    public PositionServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _mockRepository = new Mock<IPositionRepository>();
        _service = new PositionService(_mockRepository.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenPositionsExist_ReturnsPaginationWithPositions()
    {
        // Arrange
        var db = NewDb();
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);
        _mockRepository.Setup(r => r.Query()).Returns(() => db.BasicPositions);

        // Act
        var result = await _service.GetAllAsync(new GetAllPositionQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _mockRepository.Setup(r => r.GetByIdAsync(position.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        // Act
        var result = await _service.GetByIdAsync(new GetPositionByIdQuery(position.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(position.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PositionModel?)null);

        // Act
        var result = await _service.GetByIdAsync(new GetPositionByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesPosition()
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), _faker.Commerce.Department());
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<PositionModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PositionModel p, CancellationToken _) => p);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_UpdatesPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = "Old Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(position.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);
        _mockRepository.Setup(r => r.UpdateAsync(position.Id, It.IsAny<PositionModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, PositionModel p, CancellationToken _) => p);

        var command = new UpdatePositionCommand(position.Id, "Updated Name");

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PositionModel?)null);

        var command = new UpdatePositionCommand(Guid.NewGuid(), "Updated Name");

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_DeletesPosition()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(positionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(new DeletePositionCommand(positionId), Guid.NewGuid(), CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(positionId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private DefaultContext NewDb()
    {
        return new DefaultContext(_dbOptions, new TestCompanyContext());
    }
}