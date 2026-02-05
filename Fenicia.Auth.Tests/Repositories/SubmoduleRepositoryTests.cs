using Fenicia.Auth.Domains.Submodule;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class SubmoduleRepositoryTests
{
    private AuthContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase(dbName).Options;
        return new AuthContext(options);
    }

    [Test]
    public async Task GetByModuleIdAsync_ReturnsOnlySubmodulesForModule()
    {
        var ctx = CreateContext(Guid.NewGuid().ToString());
        var moduleA = Guid.NewGuid();
        var moduleB = Guid.NewGuid();

        ctx.Submodules.AddRange(new[]
        {
            new SubmoduleModel { Name = "A1", Route = "/a1", ModuleId = moduleA },
            new SubmoduleModel { Name = "A2", Route = "/a2", ModuleId = moduleA },
            new SubmoduleModel { Name = "B1", Route = "/b1", ModuleId = moduleB }
        });

        await ctx.SaveChangesAsync();

        var sut = new SubmoduleRepository(ctx);

        var res = await sut.GetByModuleIdAsync(moduleA, CancellationToken.None);

        Assert.That(res, Is.Not.Null);
        Assert.That(res.Count, Is.EqualTo(2));
        Assert.That(res.All(s => s.ModuleId == moduleA));
    }
}
