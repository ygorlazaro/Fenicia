using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

[TestFixture]
public class UpdateProjectSubtaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectSubtaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectSubtaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectSubtaskExists_UpdatesProjectSubtaskAndReturnsResponse()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = "Old Subtask Title",
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.context.ProjectSubtasks.Add(subtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(
            subtaskId,
            taskId,
            "New Subtask Title",
            true,
            5,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(subtaskId));
            Assert.That(result.Title, Is.EqualTo("New Subtask Title"));
        }
    }

    [Test]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Subtask Title",
            true,
            5,
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
        var command = new UpdateProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Subtask Title",
            true,
            5,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectSubtask()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = "Subtask 1 Title",
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = "Subtask 2 Title",
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        this.context.ProjectSubtasks.AddRange(subtask1, subtask2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(
            subtask1Id,
            taskId,
            "Updated Subtask 1 Title",
            true,
            10,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(subtask1Id));
            Assert.That(result.Title, Is.EqualTo("Updated Subtask 1 Title"));
        }

        var updatedSubtask1 = await this.context.ProjectSubtasks.FindAsync([subtask1Id], CancellationToken.None);
        var subtask2InDb = await this.context.ProjectSubtasks.FindAsync([subtask2Id], CancellationToken.None);

        Assert.That(updatedSubtask1, Is.Not.Null);
        Assert.That(subtask2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedSubtask1.Title, Is.EqualTo("Updated Subtask 1 Title"));
            Assert.That(subtask2InDb.Title, Is.EqualTo("Subtask 2 Title"));
        }
    }

    [Test]
    public async Task Handle_WithIsCompletedChange_UpdatesProjectSubtaskSuccessfully()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new Common.Data.Models.ProjectSubtaskModel
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

        var completedAt = DateTime.UtcNow;
        var command = new UpdateProjectSubtaskCommand(
            subtaskId,
            taskId,
            "Updated Title",
            true,
            3,
            completedAt);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(subtaskId));
            Assert.That(result.IsCompleted, Is.True);
            Assert.That(result.CompletedAt, Is.EqualTo(completedAt));
        }
    }
}
