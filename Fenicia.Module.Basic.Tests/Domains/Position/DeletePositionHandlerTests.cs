using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class DeletePositionHandlerTests : IDisposable
{
    public DeletePositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new DeletePositionHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly DeletePositionHandler handler;

    [Theory]
    [InlineData("Developer")]
    [InlineData("Designer")]
    [InlineData("Manager")]
    public async Task Handle_WhenPositionExists_SetsDeletedDate(string positionName)
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = positionName
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(positionId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPosition = await this.context.BasicPositions.FindAsync([positionId], CancellationToken.None);
        Assert.NotNull(deletedPosition);
        Assert.NotNull(deletedPosition.Deleted);
        Assert.InRange(deletedPosition.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenPositionDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeletePositionCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var positions = await this.context.BasicPositions.ToListAsync();
        Assert.Empty(positions);
    }

    [Fact]
    public async Task Handle_WithMultiplePositions_OnlyDeletesSpecified()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel { Id = position1Id, Name = "Developer" };
        var position2 = new PositionModel { Id = position2Id, Name = "Designer" };

        this.context.BasicPositions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(position1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPosition = await this.context.BasicPositions.FindAsync([position1Id], CancellationToken.None);
        var notDeletedPosition = await this.context.BasicPositions.FindAsync([position2Id], CancellationToken.None);

        Assert.NotNull(deletedPosition);
        Assert.NotNull(deletedPosition.Deleted);
        Assert.NotNull(notDeletedPosition);
        Assert.Null(notDeletedPosition.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeletePositionCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var positions = await this.context.BasicPositions.ToListAsync();
        Assert.Empty(positions);
    }
}
