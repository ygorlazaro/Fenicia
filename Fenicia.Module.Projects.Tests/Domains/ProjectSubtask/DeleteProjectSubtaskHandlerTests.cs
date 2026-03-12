using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class DeleteProjectSubtaskHandlerTests : IDisposable
{
    public DeleteProjectSubtaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new DeleteProjectSubtaskHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly DeleteProjectSubtaskHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectSubtaskExists_SetsDeletedDate()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.db.ProjectSubtasks.Add(subtask);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectSubtaskCommand(subtaskId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedSubtask = await this.db.ProjectSubtasks.FindAsync([
                subtaskId
            ],
            CancellationToken.None);
        Assert.NotNull(deletedSubtask);
        Assert.NotNull(deletedSubtask.Deleted);
        Assert.InRange(deletedSubtask.Deleted.Value,
            beforeDelete.AddSeconds(-1),
            DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectSubtaskCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var subtasks = await this.db.ProjectSubtasks.ToListAsync();
        Assert.Empty(subtasks);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectSubtaskCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var subtasks = await this.db.ProjectSubtasks.ToListAsync();
        Assert.Empty(subtasks);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectSubtasks_OnlyDeletesSpecified()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new ProjectSubtaskModel
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new ProjectSubtaskModel
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        this.db.ProjectSubtasks.AddRange(subtask1,
            subtask2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectSubtaskCommand(subtask1Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedSubtask = await this.db.ProjectSubtasks.FindAsync([
                subtask1Id
            ],
            CancellationToken.None);
        var notDeletedSubtask = await this.db.ProjectSubtasks.FindAsync([
                subtask2Id
            ],
            CancellationToken.None);

        Assert.NotNull(deletedSubtask);
        Assert.NotNull(deletedSubtask.Deleted);
        Assert.NotNull(notDeletedSubtask);
        Assert.Null(notDeletedSubtask.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectSubtasks_DeletesCorrectProjectSubtask()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var subtask3Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new ProjectSubtaskModel
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new ProjectSubtaskModel
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        var subtask3 = new ProjectSubtaskModel
        {
            Id = subtask3Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 3,
            CompletedAt = null
        };

        this.db.ProjectSubtasks.AddRange(subtask1,
            subtask2,
            subtask3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectSubtaskCommand(subtask2Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var subtask1InDb = await this.db.ProjectSubtasks.FindAsync([
                subtask1Id
            ],
            CancellationToken.None);
        var deletedSubtask = await this.db.ProjectSubtasks.FindAsync([
                subtask2Id
            ],
            CancellationToken.None);
        var subtask3InDb = await this.db.ProjectSubtasks.FindAsync([
                subtask3Id
            ],
            CancellationToken.None);

        Assert.NotNull(subtask1InDb);
        Assert.NotNull(deletedSubtask);
        Assert.NotNull(subtask3InDb);
        Assert.Null(subtask1InDb.Deleted);
        Assert.NotNull(deletedSubtask.Deleted);
        Assert.Null(subtask3InDb.Deleted);
    }
}
