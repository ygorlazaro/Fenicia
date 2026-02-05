using Fenicia.Auth.Domains.State;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class StateRepositoryTests
{
    [Test]
    public async Task LoadStatesAtDatabaseAsync_AddsStatesAndReturnsThem()
    {
        var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new StateRepository(context);

        var states = new List<StateModel>
        {
            new() { Name = "TestState1", Uf = "TS" },
            new() { Name = "TestState2", Uf = "TT" }
        };

        var result = await sut.LoadStatesAtDatabaseAsync(states, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));

        var fromDb = await context.States.ToListAsync();
        Assert.That(fromDb, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(fromDb.Any(s => s.Name == "TestState1"));
            Assert.That(fromDb.Any(s => s.Uf == "TT"));
        }
    }

    private static AuthContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase(dbName).Options;
        return new AuthContext(options);
    }
}
