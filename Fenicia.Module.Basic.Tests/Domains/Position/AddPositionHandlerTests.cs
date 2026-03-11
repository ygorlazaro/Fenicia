using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class AddPositionHandlerTests : IDisposable
{
    public AddPositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new AddPositionHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly AddPositionHandler handler;

    [Theory]
    [InlineData("Developer")]
    [InlineData("Designer")]
    [InlineData("Manager")]
    [InlineData("Analyst")]
    [InlineData("Director")]
    public async Task Handle_WithValidCommand_AddsPositionAndReturnsResponse(string positionName)
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), positionName);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(positionName, result.Name);
    }

    [Theory]
    [InlineData("Architect")]
    [InlineData("Consultant")]
    [InlineData("Coordinator")]
    public async Task Handle_VerifiesPositionWasSavedToDatabase(string positionName)
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), positionName);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var position = await this.db.BasicPositions.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(position);
        Assert.Equal(positionName, position.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllPositions()
    {
        // Arrange
        var command1 = new AddPositionCommand(Guid.NewGuid(), "Developer");
        var command2 = new AddPositionCommand(Guid.NewGuid(), "Designer");

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var positions = await this.db.BasicPositions.ToListAsync();
        Assert.Equal(2, positions.Count);
    }
}
