using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

[TestFixture]
public class DeletePositionHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new DeletePositionHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private DeletePositionHandler handler = null!;

    [Test]
    public async Task Handle_WhenPositionExists_SetsDeletedDate()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Developer"
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(positionId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPosition = await this.context.Positions.FindAsync([positionId], CancellationToken.None);
        Assert.That(deletedPosition, Is.Not.Null);
        Assert.That(deletedPosition.Deleted, Is.Not.Null);
        Assert.That(deletedPosition.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedPosition.Deleted, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenPositionDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeletePositionCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var positions = await this.context.Positions.ToListAsync();
        Assert.That(positions, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultiplePositions_OnlyDeletesSpecified()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel { Id = position1Id, Name = "Developer" };
        var position2 = new PositionModel { Id = position2Id, Name = "Designer" };

        this.context.Positions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(position1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPosition = await this.context.Positions.FindAsync([position1Id], CancellationToken.None);
        var notDeletedPosition = await this.context.Positions.FindAsync([position2Id], CancellationToken.None);

        Assert.That(deletedPosition, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedPosition.Deleted, Is.Not.Null);
            Assert.That(notDeletedPosition, Is.Not.Null);
        }
        Assert.That(notDeletedPosition?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeletePositionCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var positions = await this.context.Positions.ToListAsync();
        Assert.That(positions, Is.Empty);
    }
}
