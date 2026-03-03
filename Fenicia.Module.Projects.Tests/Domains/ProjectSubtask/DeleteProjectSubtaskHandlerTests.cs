using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

[TestFixture]
public class DeleteProjectSubtaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectSubtaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectSubtaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectSubtaskExists_SetsDeletedDate()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new Common.Data.Models.ProjectSubtask
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.context.ProjectSubtasks.Add(subtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectSubtaskCommand(subtaskId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSubtask = await this.context.ProjectSubtasks.FindAsync([subtaskId], CancellationToken.None);
        Assert.That(deletedSubtask, Is.Not.Null);
        Assert.That(deletedSubtask.Deleted, Is.Not.Null);
        Assert.That(deletedSubtask.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedSubtask.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectSubtaskCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var subtasks = await this.context.ProjectSubtasks.ToListAsync();
        Assert.That(subtasks, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectSubtaskCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var subtasks = await this.context.ProjectSubtasks.ToListAsync();
        Assert.That(subtasks, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjectSubtasks_OnlyDeletesSpecified()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new Common.Data.Models.ProjectSubtask
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new Common.Data.Models.ProjectSubtask
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        this.context.ProjectSubtasks.AddRange(subtask1, subtask2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectSubtaskCommand(subtask1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedSubtask = await this.context.ProjectSubtasks.FindAsync([subtask1Id], CancellationToken.None);
        var notDeletedSubtask = await this.context.ProjectSubtasks.FindAsync([subtask2Id], CancellationToken.None);

        Assert.That(deletedSubtask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedSubtask.Deleted, Is.Not.Null);
            Assert.That(notDeletedSubtask, Is.Not.Null);
        }
        Assert.That(notDeletedSubtask?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectSubtasks_DeletesCorrectProjectSubtask()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var subtask3Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new Common.Data.Models.ProjectSubtask
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new Common.Data.Models.ProjectSubtask
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        var subtask3 = new Common.Data.Models.ProjectSubtask
        {
            Id = subtask3Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 3,
            CompletedAt = null
        };

        this.context.ProjectSubtasks.AddRange(subtask1, subtask2, subtask3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectSubtaskCommand(subtask2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var subtask1InDb = await this.context.ProjectSubtasks.FindAsync([subtask1Id], CancellationToken.None);
        var deletedSubtask = await this.context.ProjectSubtasks.FindAsync([subtask2Id], CancellationToken.None);
        var subtask3InDb = await this.context.ProjectSubtasks.FindAsync([subtask3Id], CancellationToken.None);

        Assert.That(subtask1InDb, Is.Not.Null);
        Assert.That(deletedSubtask, Is.Not.Null);
        Assert.That(subtask3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(subtask1InDb.Deleted, Is.Null);
            Assert.That(deletedSubtask.Deleted, Is.Not.Null);
            Assert.That(subtask3InDb.Deleted, Is.Null);
        }
    }
}
