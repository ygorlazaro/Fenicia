using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PositionService _service;

    public PositionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new PositionRepository(_db);
        _service = new PositionService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenPositionsExist_ReturnsPaginationWithPositions()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllPositionQuery(1, 10, null, null), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetPositionByIdQuery(position.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(position.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
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

        // Act
        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_UpdatesPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(position.Id, "Updated Name");

        // Act
        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdatePositionCommand(Guid.NewGuid(), "Updated Name");

        // Act
        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_SoftDeletesPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeletePositionCommand(position.Id), _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        var deletedPosition = await _db.BasicPositions.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == position.Id);
        deletedPosition.Should().NotBeNull();
        deletedPosition!.Deleted.Should().NotBeNull();
    }
}
