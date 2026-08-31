using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class StateRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly StateRepository _repository;

    public StateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new StateRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStates()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStateExists_ReturnsState()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(state.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(state.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStateDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenStateIsValid_InsertsState()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };

        // Act
        var result = await _repository.InsertAsync(state, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenStateExists_UpdatesState()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(state.Id, state, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(state.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenStateDoesNotExist_ReturnsNull()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };

        // Act
        var result = await _repository.UpdateAsync(state.Id, state, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenStateExists_SoftDeletesState()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(state.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedState = await _db.AuthStates.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == state.Id);
        deletedState.Should().NotBeNull();
        deletedState!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingStates()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(s => s.Id == state.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenStateExists_ReturnsTrue()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(s => s.Id == state.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(s => s.Id == state.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
