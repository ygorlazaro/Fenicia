using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PositionRepository _repository;

    public PositionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new PositionRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPositions()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(position.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(position.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenPositionIsValid_InsertsPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };

        // Act
        var result = await _repository.InsertAsync(position, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_UpdatesPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(position.Id, position, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(position.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };

        // Act
        var result = await _repository.UpdateAsync(position.Id, position, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_SoftDeletesPosition()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(position.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedPosition = await _db.BasicPositions.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == position.Id);
        deletedPosition.Should().NotBeNull();
        deletedPosition!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingPositions()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(p => p.Id == position.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenPositionExists_ReturnsTrue()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(p => p.Id == position.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(p => p.Id == position.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
