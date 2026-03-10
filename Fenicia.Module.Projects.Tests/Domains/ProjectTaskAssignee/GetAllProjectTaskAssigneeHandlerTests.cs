using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class GetAllProjectTaskAssigneeHandlerTests : IDisposable
{
    public GetAllProjectTaskAssigneeHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new GetAllProjectTaskAssigneeHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly GetAllProjectTaskAssigneeHandler handler;

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectTaskAssigneeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectTaskAssignees_ReturnsAllProjectTaskAssignees()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var assignee1 = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId1,
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow.AddDays(-5)
        };

        var assignee2 = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId2,
            Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-3)
        };

        this.context.ProjectTaskAssignees.AddRange(assignee1, assignee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(assignee1.Id, result[0].Id);
        Assert.Equal(assignee2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var assignee = new TaskAssigneeModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = Guid.NewGuid(),
                Role = i % 2 == 0 ? Common.Enums.Project.EnumAssigneeRole.Owner : Common.Enums.Project.EnumAssigneeRole.Contributor,
                AssignedAt = DateTime.UtcNow.AddDays(-i)
            };
            this.context.ProjectTaskAssignees.Add(assignee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var assignee = new TaskAssigneeModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = Guid.NewGuid(),
                Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
                AssignedAt = DateTime.UtcNow.AddDays(-i)
            };
            this.context.ProjectTaskAssignees.Add(assignee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var assignee = new TaskAssigneeModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = Guid.NewGuid(),
                Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
                AssignedAt = DateTime.UtcNow.AddDays(-i)
            };
            this.context.ProjectTaskAssignees.Add(assignee);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskAssigneeQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}
