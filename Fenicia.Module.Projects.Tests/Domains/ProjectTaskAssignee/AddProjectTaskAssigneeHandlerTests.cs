using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class AddProjectTaskAssigneeHandlerTests : IDisposable
{
    public AddProjectTaskAssigneeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new AddProjectTaskAssigneeHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly AddProjectTaskAssigneeHandler handler;

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectTaskAssigneeAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Role, result.Role);
    }

    [Fact]
    public async Task Handle_VerifiesProjectTaskAssigneeWasSaved()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            taskId,
            userId,
            "Owner",
            assignedAt);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignee = await this.context.ProjectTaskAssignees
            .FirstOrDefaultAsync(a => a.Id == command.Id);

        Assert.NotNull(assignee);
        Assert.Equal(taskId, assignee.TaskId);
        Assert.Equal(userId, assignee.UserId);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectTaskAssignees()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command1 = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            taskId,
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow.AddDays(-5));

        var command2 = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            taskId,
            Guid.NewGuid(),
            "Contributor",
            DateTime.UtcNow.AddDays(-3));

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var assignees = await this.context.ProjectTaskAssignees.ToListAsync();
        Assert.Equal(2, assignees.Count);
    }

    [Fact]
    public async Task Handle_WithMemberRole_AddsProjectTaskAssigneeSuccessfully()
    {
        // Arrange
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Contributor",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal("Contributor", result.Role);
    }

    [Fact]
    public async Task Handle_WithPastAssignedDate_AddsProjectTaskAssigneeSuccessfully()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-30);
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            pastDate);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(pastDate, result.AssignedAt);
    }
}
