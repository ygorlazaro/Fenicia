using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class DeletePositionHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly DeletePositionHandler handler;

    public DeletePositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeletePositionHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData("Developer")]
    [InlineData("Designer")]
    [InlineData("Manager")]
    public async Task Handle_WhenPositionExists_SetsDeletedDate(string positionName)
    {

        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = positionName
        };

        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(positionId);
        var beforeDelete = DateTime.Now;

        await handler.Handle(command, CancellationToken.None);

        var deletedPosition = await db.BasicPositions.FindAsync([positionId], CancellationToken.None);
        Assert.NotNull(deletedPosition);
        Assert.NotNull(deletedPosition.Deleted);
        Assert.InRange(deletedPosition.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenPositionDoesNotExist_DoesNothing()
    {

        var command = new DeletePositionCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var positions = await db.BasicPositions.ToListAsync();
        Assert.Empty(positions);
    }

    [Fact]
    public async Task Handle_WithMultiplePositions_OnlyDeletesSpecified()
    {

        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel
        {
            Id = position1Id,
            Name = "Developer"
        };
        var position2 = new PositionModel
        {
            Id = position2Id,
            Name = "Designer"
        };

        db.BasicPositions.AddRange(position1, position2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(position1Id);

        await handler.Handle(command, CancellationToken.None);

        var deletedPosition = await db.BasicPositions.FindAsync([position1Id], CancellationToken.None);
        var notDeletedPosition = await db.BasicPositions.FindAsync([position2Id], CancellationToken.None);

        Assert.NotNull(deletedPosition);
        Assert.NotNull(deletedPosition.Deleted);
        Assert.NotNull(notDeletedPosition);
        Assert.Null(notDeletedPosition.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {

        var command = new DeletePositionCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var positions = await db.BasicPositions.ToListAsync();
        Assert.Empty(positions);
    }
}
