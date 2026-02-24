using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

[TestFixture]
public class AddPositionHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddPositionHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddPositionHandler handler = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsPositionAndReturnsResponse()
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), "Developer");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Name, Is.EqualTo(command.Name));
        }
    }

    [Test]
    public async Task Handle_VerifiesPositionWasSavedToDatabase()
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), "Designer");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var position = await this.context.Positions.FindAsync([command.Id], CancellationToken.None);
        Assert.That(position, Is.Not.Null);
        Assert.That(position.Name, Is.EqualTo(command.Name));
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllPositions()
    {
        // Arrange
        var command1 = new AddPositionCommand(Guid.NewGuid(), "Developer");
        var command2 = new AddPositionCommand(Guid.NewGuid(), "Designer");

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var positions = await this.context.Positions.ToListAsync();
        Assert.That(positions, Has.Count.EqualTo(2));
    }
}
