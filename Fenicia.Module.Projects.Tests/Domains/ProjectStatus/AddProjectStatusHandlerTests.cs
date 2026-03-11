using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectStatus.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class AddProjectStatusHandlerTests : IDisposable
{
    public AddProjectStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new AddProjectStatusHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly AddProjectStatusHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectStatusAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            this.faker.Random.Number(0, 100),
            this.faker.Random.Bool());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
    }

    [Fact]
    public async Task Handle_VerifiesProjectStatusWasSaved()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            this.faker.Random.Number(0, 100),
            this.faker.Random.Bool());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var status = await this.db.ProjectStatuses
            .FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.NotNull(status);
        Assert.Equal(command.Name, status.Name);
        Assert.Equal(command.Color, status.Color);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectStatuses()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command1 = new AddProjectStatusCommand(
            Guid.NewGuid(),
            projectId,
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            1,
            false);

        var command2 = new AddProjectStatusCommand(
            Guid.NewGuid(),
            projectId,
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            2,
            true);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var statuses = await this.db.ProjectStatuses.ToListAsync();
        Assert.Equal(2, statuses.Count);
    }

    [Fact]
    public async Task Handle_WithIsFinalTrue_AddsProjectStatusSuccessfully()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            10,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.True(result.IsFinal);
    }

    [Fact]
    public async Task Handle_WithOrderZero_AddsProjectStatusSuccessfully()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            0,
            false);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(0, result.Order);
    }
}
