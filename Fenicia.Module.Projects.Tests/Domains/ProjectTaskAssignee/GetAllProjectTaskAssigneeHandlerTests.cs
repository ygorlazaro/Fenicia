using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

[TestFixture]
public class GetAllProjectTaskAssigneeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectTaskAssigneeHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectTaskAssigneeHandler handler = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectTaskAssigneeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjectTaskAssignees_ReturnsAllProjectTaskAssignees()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var assignee1 = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId1,
            Role = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId2,
            Role = Common.Enums.Project.AssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-3)
        };

        this.context.ProjectTaskAssignees.AddRange(assignee1, assignee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(assignee1.Id));
            Assert.That(result[1].Id, Is.EqualTo(assignee2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = Guid.NewGuid(),
                Role = i % 2 == 0 ? Common.Enums.Project.AssigneeRole.Owner : Common.Enums.Project.AssigneeRole.Contributor,
                AssignedAt = DateTime.UtcNow.AddDays(-i)
            };
            this.context.ProjectTaskAssignees.Add(assignee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = Guid.NewGuid(),
                Role = Common.Enums.Project.AssigneeRole.Contributor,
                AssignedAt = DateTime.UtcNow.AddDays(-i)
            };
            this.context.ProjectTaskAssignees.Add(assignee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = Guid.NewGuid(),
                Role = Common.Enums.Project.AssigneeRole.Contributor,
                AssignedAt = DateTime.UtcNow.AddDays(-i)
            };
            this.context.ProjectTaskAssignees.Add(assignee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
