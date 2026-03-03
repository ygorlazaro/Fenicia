using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

[TestFixture]
public class DeleteProjectTaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectTaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectTaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectTaskExists_SetsDeletedDate()
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

        var command = new DeleteProjectTaskCommand(taskId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedTask = await this.context.ProjectTasks.FindAsync([taskId], CancellationToken.None);
        Assert.That(deletedTask, Is.Not.Null);
        Assert.That(deletedTask.Deleted, Is.Not.Null);
        Assert.That(deletedTask.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedTask.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectTaskDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var tasks = await this.context.ProjectTasks.ToListAsync();
        Assert.That(tasks, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var tasks = await this.context.ProjectTasks.ToListAsync();
        Assert.That(tasks, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjectTasks_OnlyDeletesSpecified()
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
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
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
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Low,
            Type = Common.Enums.Project.TaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.AddRange(task1, task2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectTaskCommand(task1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedTask = await this.context.ProjectTasks.FindAsync([task1Id], CancellationToken.None);
        var notDeletedTask = await this.context.ProjectTasks.FindAsync([task2Id], CancellationToken.None);

        Assert.That(deletedTask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedTask.Deleted, Is.Not.Null);
            Assert.That(notDeletedTask, Is.Not.Null);
        }
        Assert.That(notDeletedTask?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectTasks_DeletesCorrectProjectTask()
    {
        // Arrange
        var task1Id = Guid.NewGuid();
        var task2Id = Guid.NewGuid();
        var task3Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();

        var task1 = new Common.Data.Models.ProjectTask
        {
            Id = task1Id,
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

        var task2 = new Common.Data.Models.ProjectTask
        {
            Id = task2Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Low,
            Type = Common.Enums.Project.TaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        var task3 = new Common.Data.Models.ProjectTask
        {
            Id = task3Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.High,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 3,
            EstimatePoints = 8,
            DueDate = DateTime.UtcNow.AddDays(14),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.AddRange(task1, task2, task3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectTaskCommand(task2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var task1InDb = await this.context.ProjectTasks.FindAsync([task1Id], CancellationToken.None);
        var deletedTask = await this.context.ProjectTasks.FindAsync([task2Id], CancellationToken.None);
        var task3InDb = await this.context.ProjectTasks.FindAsync([task3Id], CancellationToken.None);

        Assert.That(task1InDb, Is.Not.Null);
        Assert.That(deletedTask, Is.Not.Null);
        Assert.That(task3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(task1InDb.Deleted, Is.Null);
            Assert.That(deletedTask.Deleted, Is.Not.Null);
            Assert.That(task3InDb.Deleted, Is.Null);
        }
    }
}
