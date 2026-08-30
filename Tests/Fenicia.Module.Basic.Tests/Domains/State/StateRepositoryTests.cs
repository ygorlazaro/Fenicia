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
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStateExists_ReturnsState()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(state.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(state.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStateDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenStateIsValid_InsertsState()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };

        var result = await _repository.InsertAsync(state, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenStateExists_UpdatesState()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(state.Id, state, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(state.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenStateDoesNotExist_ReturnsNull()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };

        var result = await _repository.UpdateAsync(state.Id, state, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenStateExists_SoftDeletesState()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(state.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedState = await _db.AuthStates.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == state.Id);
        Assert.NotNull(deletedState);
        Assert.NotNull(deletedState.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingStates()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(s => s.Id == state.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenStateExists_ReturnsTrue()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(s => s.Id == state.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(s => s.Id == state.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
