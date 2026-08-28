using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class DeletePositionServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly PositionService service;

    public DeletePositionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new PositionService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_SetsDeletedDate()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeletePositionCommand(position.Id), CancellationToken.None);

        var updated = await db.BasicPositions.FindAsync(position.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionDoesNotExist_DoesNothing()
    {
        await service.DeleteAsync(new DeletePositionCommand(Guid.NewGuid()), CancellationToken.None);

        var count = await db.BasicPositions.CountAsync();
        Assert.Equal(0, count);
    }
}
