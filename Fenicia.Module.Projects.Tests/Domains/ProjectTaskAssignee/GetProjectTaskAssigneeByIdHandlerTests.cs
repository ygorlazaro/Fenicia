using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

[TestFixture]
public class GetProjectTaskAssigneeByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProjectTaskAssigneeByIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProjectTaskAssigneeByIdHandler handler = null!;

    [Test]
    public async Task Handle_WhenProjectTaskAssigneeExists_ReturnsProjectTaskAssigneeResponse()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow.AddDays(-5);
        var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = assignedAt
        };

        this.context.ProjectTaskAssignees.Add(assignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskAssigneeByIdQuery(assigneeId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(assigneeId));
            Assert.That(result.UserId, Is.EqualTo(userId));
        }
    }

    [Test]
    public async Task Handle_WhenProjectTaskAssigneeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectTaskAssigneeByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectTaskAssigneeByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectTaskAssignees_ReturnsOnlyRequestedAssignee()
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

        var query = new GetProjectTaskAssigneeByIdQuery(assignee1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(assignee1Id));
            Assert.That(result.UserId, Is.EqualTo(userId1));
        }
    }

    [Test]
    public async Task Handle_WithMemberRole_ReturnsCorrectResponse()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow.AddDays(-10);
        var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
            AssignedAt = assignedAt
        };

        this.context.ProjectTaskAssignees.Add(assignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskAssigneeByIdQuery(assigneeId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(assigneeId));
            Assert.That(result.Role, Is.EqualTo("Contributor"));
        }
    }
}
