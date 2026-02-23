using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

[TestFixture]
public class UpdatePositionHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new UpdatePositionHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private UpdatePositionHandler handler = null!;

    [Test]
    public async Task Handle_WhenPositionExists_UpdatesPositionAndReturnsResponse()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Old Position"
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(positionId, "New Position");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(positionId));
            Assert.That(result.Name, Is.EqualTo("New Position"));
        }
    }

    [Test]
    public async Task Handle_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdatePositionCommand(Guid.NewGuid(), "New Position");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdatePositionCommand(Guid.NewGuid(), "New Position");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesPositionWasUpdatedInDatabase()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Old Position"
        };

        this.context.Positions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(positionId, "New Position");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedPosition = await this.context.Positions.FindAsync([positionId], CancellationToken.None);
        Assert.That(updatedPosition, Is.Not.Null);
        Assert.That(updatedPosition.Name, Is.EqualTo("New Position"));
    }

    [Test]
    public async Task Handle_WithMultiplePositions_OnlyUpdatesSpecified()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel { Id = position1Id, Name = "Developer" };
        var position2 = new PositionModel { Id = position2Id, Name = "Designer" };

        this.context.Positions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(position1Id, "Senior Developer");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedPosition1 = await this.context.Positions.FindAsync([position1Id], CancellationToken.None);
        var notUpdatedPosition2 = await this.context.Positions.FindAsync([position2Id], CancellationToken.None);

        Assert.That(updatedPosition1, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedPosition1.Name, Is.EqualTo("Senior Developer"));
            Assert.That(notUpdatedPosition2, Is.Not.Null);
        }
        Assert.That(notUpdatedPosition2.Name, Is.EqualTo("Designer"));
    }
}
