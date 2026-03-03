using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

[TestFixture]
public class UpdateProjectTaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectTaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectTaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectTaskExists_UpdatesProjectTaskAndReturnsResponse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task = new Common.Data.Models.ProjectTask
        {
            Id = taskId,
            ProjectId = projectId,
            StatusId = statusId,
            Title = "Old Task Title",
            Description = "Old Description",
            Priority = Common.Enums.Project.TaskPriority.Low,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 3,
            DueDate = DateTime.UtcNow.AddDays(5),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(task);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(
            taskId,
            projectId,
            statusId,
            "New Task Title",
            "New Description",
            "High",
            "Bug",
            10,
            8,
            DateTime.UtcNow.AddDays(14),
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(taskId));
            Assert.That(result.Title, Is.EqualTo("New Task Title"));
        }
    }

    [Test]
    public async Task Handle_WhenProjectTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Task Title",
            "New Description",
            "High",
            "Bug",
            1,
            5,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Task Title",
            "New Description",
            "High",
            "Bug",
            1,
            5,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectTask()
    {
        // Arrange
        var task1Id = Guid.NewGuid();
        var task2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();

        var task1 = new Common.Data.Models.ProjectTask
        {
            Id = task1Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = "Task 1 Title",
            Description = "Task 1 Description",
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        var task2 = new Common.Data.Models.ProjectTask
        {
            Id = task2Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = "Task 2 Title",
            Description = "Task 2 Description",
            Priority = Common.Enums.Project.TaskPriority.Low,
            Type = Common.Enums.Project.TaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.AddRange(task1, task2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(
            task1Id,
            projectId,
            statusId,
            "Updated Task 1 Title",
            "Updated Task 1 Description",
            "High",
            "Task",
            5,
            10,
            DateTime.UtcNow.AddDays(21),
            task1.CreatedBy);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(task1Id));
            Assert.That(result.Title, Is.EqualTo("Updated Task 1 Title"));
        }

        var updatedTask1 = await this.context.ProjectTasks.FindAsync([task1Id], CancellationToken.None);
        var task2InDb = await this.context.ProjectTasks.FindAsync([task2Id], CancellationToken.None);

        Assert.That(updatedTask1, Is.Not.Null);
        Assert.That(task2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedTask1.Title, Is.EqualTo("Updated Task 1 Title"));
            Assert.That(task2InDb.Title, Is.EqualTo("Task 2 Title"));
        }
    }

    [Test]
    public async Task Handle_WithNullDescription_UpdatesProjectTaskSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task = new Common.Data.Models.ProjectTask
        {
            Id = taskId,
            ProjectId = projectId,
            StatusId = statusId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(task);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(
            taskId,
            projectId,
            statusId,
            "Updated Title",
            null,
            "Medium",
            "Task",
            1,
            5,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(taskId));
            Assert.That(result.Description, Is.Null);
        }
    }
}
