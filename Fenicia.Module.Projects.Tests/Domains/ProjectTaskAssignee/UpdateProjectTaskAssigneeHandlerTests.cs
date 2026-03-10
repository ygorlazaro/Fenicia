using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class UpdateProjectTaskAssigneeHandlerTests : IDisposable
{
    public UpdateProjectTaskAssigneeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new UpdateProjectTaskAssigneeHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly UpdateProjectTaskAssigneeHandler handler;

    [Fact]
    public async Task Handle_WhenProjectTaskAssigneeExists_UpdatesProjectTaskAssigneeAndReturnsResponse()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignee = new TaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-10)
        };

        this.context.ProjectTaskAssignees.Add(assignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var newUserId = Guid.NewGuid();
        var command = new UpdateProjectTaskAssigneeCommand(
            assigneeId,
            taskId,
            newUserId,
            "Owner",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assigneeId, result.Id);
        Assert.Equal("Owner", result.Role);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskAssigneeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectTaskAssignee()
    {
        // Arrange
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var assignee1 = new TaskAssigneeModel
        {
            Id = assignee1Id,
            TaskId = taskId,
            UserId = userId1,
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new TaskAssigneeModel
        {
            Id = assignee2Id,
            TaskId = taskId,
            UserId = userId2,
            Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-3)
        };

        this.context.ProjectTaskAssignees.AddRange(assignee1, assignee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var newUserId = Guid.NewGuid();
        var command = new UpdateProjectTaskAssigneeCommand(
            assignee1Id,
            taskId,
            newUserId,
            "Contributor",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assignee1Id, result.Id);
        Assert.Equal("Contributor", result.Role);

        var updatedAssignee1 = await this.context.ProjectTaskAssignees.FindAsync([assignee1Id], CancellationToken.None);
        var assignee2InDb = await this.context.ProjectTaskAssignees.FindAsync([assignee2Id], CancellationToken.None);

        Assert.NotNull(updatedAssignee1);
        Assert.NotNull(assignee2InDb);
        Assert.Equal(Common.Enums.Project.EnumAssigneeRole.Contributor, updatedAssignee1.Role);
        Assert.Equal(Common.Enums.Project.EnumAssigneeRole.Contributor, assignee2InDb.Role);
    }

    [Fact]
    public async Task Handle_WithRoleChange_UpdatesProjectTaskAssigneeSuccessfully()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignee = new TaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-10)
        };

        this.context.ProjectTaskAssignees.Add(assignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskAssigneeCommand(
            assigneeId,
            taskId,
            userId,
            "Owner",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assigneeId, result.Id);
        Assert.Equal("Owner", result.Role);
    }
}
