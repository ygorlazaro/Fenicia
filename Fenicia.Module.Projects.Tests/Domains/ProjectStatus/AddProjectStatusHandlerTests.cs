using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class AddProjectStatusHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddProjectStatusHandler handler;

    public AddProjectStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddProjectStatusHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectStatusAndReturnsResponse()
    {

        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Word(), faker.Internet.Color(), faker.Random.Number(0, 100), faker.Random.Bool());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
    }

    [Fact]
    public async Task Handle_VerifiesProjectStatusWasSaved()
    {

        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Word(), faker.Internet.Color(), faker.Random.Number(0, 100), faker.Random.Bool());

        await handler.Handle(command, CancellationToken.None);

        var status = await db.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.NotNull(status);
        Assert.Equal(command.Name, status.Name);
        Assert.Equal(command.Color, status.Color);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectStatuses()
    {

        var projectId = Guid.NewGuid();
        var command1 = new AddProjectStatusCommand(Guid.NewGuid(), projectId, faker.Lorem.Word(), faker.Internet.Color(), 1, false);

        var command2 = new AddProjectStatusCommand(Guid.NewGuid(), projectId, faker.Lorem.Word(), faker.Internet.Color(), 2, true);

        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        var statuses = await db.ProjectStatuses.ToListAsync();
        Assert.Equal(2, statuses.Count);
    }

    [Fact]
    public async Task Handle_WithIsFinalTrue_AddsProjectStatusSuccessfully()
    {

        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Word(), faker.Internet.Color(), 10, true);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.True(result.IsFinal);
    }

    [Fact]
    public async Task Handle_WithOrderZero_AddsProjectStatusSuccessfully()
    {

        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Word(), faker.Internet.Color(), 0, false);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(0, result.Order);
    }
}
