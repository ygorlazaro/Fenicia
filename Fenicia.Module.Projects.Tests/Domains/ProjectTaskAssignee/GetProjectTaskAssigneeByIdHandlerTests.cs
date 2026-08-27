using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class GetProjectTaskAssigneeByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetProjectTaskAssigneeByIdHandler handler;

    public GetProjectTaskAssigneeByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectTaskAssigneeByIdHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskAssigneeExists_ReturnsProjectTaskAssigneeResponse()
    {

        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow.AddDays(-5);
        var assignee = new TaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = EnumAssigneeRole.Owner,
            AssignedAt = assignedAt
        };

        db.ProjectTaskAssignees.Add(assignee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskAssigneeByIdQuery(assigneeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(assigneeId, result.Id);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskAssigneeDoesNotExist_ReturnsNull()
    {

        var query = new GetProjectTaskAssigneeByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetProjectTaskAssigneeByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTaskAssignees_ReturnsOnlyRequestedAssignee()
    {

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
            Role = EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new TaskAssigneeModel
        {
            Id = assignee2Id,
            TaskId = taskId,
            UserId = userId2,
            Role = EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-3)
        };

        db.ProjectTaskAssignees.AddRange(assignee1, assignee2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskAssigneeByIdQuery(assignee1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(assignee1Id, result.Id);
        Assert.Equal(userId1, result.UserId);
    }

    [Fact]
    public async Task Handle_WithMemberRole_ReturnsCorrectResponse()
    {

        var assigneeId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow.AddDays(-10);
        var assignee = new TaskAssigneeModel
        {
            Id = assigneeId,
            TaskId = taskId,
            UserId = userId,
            Role = EnumAssigneeRole.Contributor,
            AssignedAt = assignedAt
        };

        db.ProjectTaskAssignees.Add(assignee);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskAssigneeByIdQuery(assigneeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(assigneeId, result.Id);
        Assert.Equal("Contributor", result.Role);
    }
}
