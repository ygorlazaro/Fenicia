using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class AddPositionHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly AddPositionHandler handler;

    public AddPositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddPositionHandler(db);
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
    [InlineData("Analyst")]
    [InlineData("Director")]
    public async Task Handle_WithValidCommand_AddsPositionAndReturnsResponse(string positionName)
    {

        var command = new AddPositionCommand(Guid.NewGuid(), positionName);

        var result = await handler.Handle(command, CancellationToken.None);

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

        var command = new AddPositionCommand(Guid.NewGuid(), positionName);

        await handler.Handle(command, CancellationToken.None);

        var position = await db.BasicPositions.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(position);
        Assert.Equal(positionName, position.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllPositions()
    {

        var command1 = new AddPositionCommand(Guid.NewGuid(), "Developer");
        var command2 = new AddPositionCommand(Guid.NewGuid(), "Designer");

        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        var positions = await db.BasicPositions.ToListAsync();
        Assert.Equal(2, positions.Count);
    }
}
