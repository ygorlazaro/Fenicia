using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class UpdatePositionHandlerTests : IDisposable
{
    public UpdatePositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new UpdatePositionHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly UpdatePositionHandler handler;

    [Theory]
    [InlineData("Old Position", "New Position")]
    [InlineData("Developer", "Senior Developer")]
    [InlineData("Junior", "Pleno")]
    [InlineData("Analyst", "Coordinator")]
    public async Task Handle_WhenPositionExists_UpdatesPositionAndReturnsResponse(string oldName, string newName)
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = oldName
        };

        this.db.BasicPositions.Add(position);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(positionId,
            newName);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(positionId,
            result.Id);
        Assert.Equal(newName,
            result.Name);
    }

    [Fact]
    public async Task Handle_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdatePositionCommand(Guid.NewGuid(),
            "New Position");

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdatePositionCommand(Guid.NewGuid(),
            "New Position");

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesPositionWasUpdatedInDatabase()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel
        {
            Id = positionId,
            Name = "Old Position"
        };

        this.db.BasicPositions.Add(position);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(positionId,
            "New Position");

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var updatedPosition = await this.db.BasicPositions.FindAsync([
                positionId
            ],
            CancellationToken.None);
        Assert.NotNull(updatedPosition);
        Assert.Equal("New Position",
            updatedPosition.Name);
    }

    [Fact]
    public async Task Handle_WithMultiplePositions_OnlyUpdatesSpecified()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel { Id = position1Id, Name = "Developer" };
        var position2 = new PositionModel { Id = position2Id, Name = "Designer" };

        this.db.BasicPositions.AddRange(position1,
            position2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(position1Id,
            "Senior Developer");

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var updatedPosition1 = await this.db.BasicPositions.FindAsync([
                position1Id
            ],
            CancellationToken.None);
        var notUpdatedPosition2 = await this.db.BasicPositions.FindAsync([
                position2Id
            ],
            CancellationToken.None);

        Assert.NotNull(updatedPosition1);
        Assert.Equal("Senior Developer",
            updatedPosition1.Name);
        Assert.NotNull(notUpdatedPosition2);
        Assert.Equal("Designer",
            notUpdatedPosition2.Name);
    }
}
