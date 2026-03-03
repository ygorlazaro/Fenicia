using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

[TestFixture]
public class UpdateProjectTaskAssigneeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectTaskAssigneeHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectTaskAssigneeHandler handler = null!;

    [Test]
    public async Task Handle_WhenProjectTaskAssigneeExists_UpdatesProjectTaskAssigneeAndReturnsResponse()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
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
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(assigneeId));
            Assert.That(result.Role, Is.EqualTo("Owner"));
        }
    }

    [Test]
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
        Assert.That(result, Is.Null);
    }

    [Test]
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
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectTaskAssignee()
    {
        // Arrange
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var assignee1 = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = assignee1Id,
            TaskId = taskId,
            UserId = userId1,
            Role = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = assignee2Id,
            TaskId = taskId,
            UserId = userId2,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
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
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(assignee1Id));
            Assert.That(result.Role, Is.EqualTo("Contributor"));
        }

        var updatedAssignee1 = await this.context.ProjectTaskAssignees.FindAsync([assignee1Id], CancellationToken.None);
        var assignee2InDb = await this.context.ProjectTaskAssignees.FindAsync([assignee2Id], CancellationToken.None);

        Assert.That(updatedAssignee1, Is.Not.Null);
        Assert.That(assignee2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedAssignee1.Role, Is.EqualTo(Common.Enums.Project.AssigneeRole.Contributor));
            Assert.That(assignee2InDb.Role, Is.EqualTo(Common.Enums.Project.AssigneeRole.Contributor));
        }
    }

    [Test]
    public async Task Handle_WithRoleChange_UpdatesProjectTaskAssigneeSuccessfully()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
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
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(assigneeId));
            Assert.That(result.Role, Is.EqualTo("Owner"));
        }
    }
}
