using Fenicia.Auth.Domains.State;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class StateRepositoryTests
{
    private AuthContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase(dbName).Options;
        return new AuthContext(options);
    }

    [Test]
    public async Task LoadStatesAtDatabaseAsync_AddsStatesAndReturnsThem()
    {
        var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new StateRepository(context);

        var states = new List<StateModel>
        {
            new StateModel { Name = "TestState1", Uf = "TS" },
            new StateModel { Name = "TestState2", Uf = "TT" }
        };

        var result = await sut.LoadStatesAtDatabaseAsync(states, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));

        // Ensure persisted in the context
        var fromDb = await context.States.ToListAsync();
        Assert.That(fromDb.Count, Is.EqualTo(2));
        Assert.That(fromDb.Any(s => s.Name == "TestState1"));
        Assert.That(fromDb.Any(s => s.Uf == "TT"));
    }
}
