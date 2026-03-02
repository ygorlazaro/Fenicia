using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Position.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

[TestFixture]
public class DeletePositionHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeletePositionHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeletePositionHandler handler = null!;

    [Test]
    public async Task Handle_WhenPositionExists_SetsDeletedDate()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new BasicPosition
        {
            Id = positionId,
            Name = "Developer"
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(positionId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPosition = await this.context.BasicPositions.FindAsync([positionId], CancellationToken.None);
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
        var positions = await this.context.BasicPositions.ToListAsync();
        Assert.That(positions, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultiplePositions_OnlyDeletesSpecified()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new BasicPosition { Id = position1Id, Name = "Developer" };
        var position2 = new BasicPosition { Id = position2Id, Name = "Designer" };

        this.context.BasicPositions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeletePositionCommand(position1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPosition = await this.context.BasicPositions.FindAsync([position1Id], CancellationToken.None);
        var notDeletedPosition = await this.context.BasicPositions.FindAsync([position2Id], CancellationToken.None);

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
        var positions = await this.context.BasicPositions.ToListAsync();
        Assert.That(positions, Is.Empty);
    }
}
