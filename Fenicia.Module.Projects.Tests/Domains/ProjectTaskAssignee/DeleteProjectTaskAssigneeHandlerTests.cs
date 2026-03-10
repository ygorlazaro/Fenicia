using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class DeleteProjectTaskAssigneeHandlerTests : IDisposable
{
    public DeleteProjectTaskAssigneeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new DeleteProjectTaskAssigneeHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly DeleteProjectTaskAssigneeHandler handler;

    [Fact]
    public async Task Handle_WhenProjectTaskAssigneeExists_SetsDeletedDate()
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
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        this.context.ProjectTaskAssignees.Add(assignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectTaskAssigneeCommand(assigneeId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedAssignee = await this.context.ProjectTaskAssignees.FindAsync([assigneeId], CancellationToken.None);
        Assert.NotNull(deletedAssignee);
        Assert.NotNull(deletedAssignee.Deleted);
        Assert.InRange(deletedAssignee.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectTaskAssigneeDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskAssigneeCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignees = await this.context.ProjectTaskAssignees.ToListAsync();
        Assert.Empty(assignees);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskAssigneeCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignees = await this.context.ProjectTaskAssignees.ToListAsync();
        Assert.Empty(assignees);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTaskAssignees_OnlyDeletesSpecified()
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

        var command = new DeleteProjectTaskAssigneeCommand(assignee1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedAssignee = await this.context.ProjectTaskAssignees.FindAsync([assignee1Id], CancellationToken.None);
        var notDeletedAssignee = await this.context.ProjectTaskAssignees.FindAsync([assignee2Id], CancellationToken.None);

        Assert.NotNull(deletedAssignee);
        Assert.NotNull(deletedAssignee.Deleted);
        Assert.NotNull(notDeletedAssignee);
        Assert.Null(notDeletedAssignee.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTaskAssignees_DeletesCorrectProjectTaskAssignee()
    {
        // Arrange
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();
        var assignee3Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var userId3 = Guid.NewGuid();

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

        var assignee3 = new TaskAssigneeModel
        {
            Id = assignee3Id,
            TaskId = taskId,
            UserId = userId3,
            Role = Common.Enums.Project.EnumAssigneeRole.Reviewer,
            AssignedAt = DateTime.UtcNow.AddDays(-1)
        };

        this.context.ProjectTaskAssignees.AddRange(assignee1, assignee2, assignee3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectTaskAssigneeCommand(assignee2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignee1InDb = await this.context.ProjectTaskAssignees.FindAsync([assignee1Id], CancellationToken.None);
        var deletedAssignee = await this.context.ProjectTaskAssignees.FindAsync([assignee2Id], CancellationToken.None);
        var assignee3InDb = await this.context.ProjectTaskAssignees.FindAsync([assignee3Id], CancellationToken.None);

        Assert.NotNull(assignee1InDb);
        Assert.NotNull(deletedAssignee);
        Assert.NotNull(assignee3InDb);
        Assert.Null(assignee1InDb.Deleted);
        Assert.NotNull(deletedAssignee.Deleted);
        Assert.Null(assignee3InDb.Deleted);
    }
}
