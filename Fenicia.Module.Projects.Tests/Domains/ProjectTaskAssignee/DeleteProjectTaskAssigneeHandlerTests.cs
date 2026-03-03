using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

[TestFixture]
public class DeleteProjectTaskAssigneeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectTaskAssigneeHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectTaskAssigneeHandler handler = null!;

    [Test]
    public async Task Handle_WhenProjectTaskAssigneeExists_SetsDeletedDate()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignee = new Common.Data.Models.ProjectTaskAssignee
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.AssigneeRole.Owner,
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
        Assert.That(deletedAssignee, Is.Not.Null);
        Assert.That(deletedAssignee.Deleted, Is.Not.Null);
        Assert.That(deletedAssignee.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedAssignee.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectTaskAssigneeDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskAssigneeCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignees = await this.context.ProjectTaskAssignees.ToListAsync();
        Assert.That(assignees, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskAssigneeCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignees = await this.context.ProjectTaskAssignees.ToListAsync();
        Assert.That(assignees, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjectTaskAssignees_OnlyDeletesSpecified()
    {
        // Arrange
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var assignee1 = new Common.Data.Models.ProjectTaskAssignee
        {
            Id = assignee1Id,
            TaskId = taskId,
            UserId = userId1,
            Role = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new Common.Data.Models.ProjectTaskAssignee
        {
            Id = assignee2Id,
            TaskId = taskId,
            UserId = userId2,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
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

        Assert.That(deletedAssignee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedAssignee.Deleted, Is.Not.Null);
            Assert.That(notDeletedAssignee, Is.Not.Null);
        }
        Assert.That(notDeletedAssignee?.Deleted, Is.Null);
    }

    [Test]
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

        var assignee1 = new Common.Data.Models.ProjectTaskAssignee
        {
            Id = assignee1Id,
            TaskId = taskId,
            UserId = userId1,
            Role = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new Common.Data.Models.ProjectTaskAssignee
        {
            Id = assignee2Id,
            TaskId = taskId,
            UserId = userId2,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-3)
        };

        var assignee3 = new Common.Data.Models.ProjectTaskAssignee
        {
            Id = assignee3Id,
            TaskId = taskId,
            UserId = userId3,
            Role = Common.Enums.Project.AssigneeRole.Reviewer,
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

        Assert.That(assignee1InDb, Is.Not.Null);
        Assert.That(deletedAssignee, Is.Not.Null);
        Assert.That(assignee3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(assignee1InDb.Deleted, Is.Null);
            Assert.That(deletedAssignee.Deleted, Is.Not.Null);
            Assert.That(assignee3InDb.Deleted, Is.Null);
        }
    }
}
