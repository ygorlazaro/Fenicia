using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class StateServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly StateService _service;

    public StateServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new StateRepository(_db);
        _service = new StateService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatesExist_ReturnsStates()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = _faker.Address.State(), Uf = "SP" };
        _db.AuthStates.Add(state);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllStateQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoStatesExist_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync(new GetAllStateQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
