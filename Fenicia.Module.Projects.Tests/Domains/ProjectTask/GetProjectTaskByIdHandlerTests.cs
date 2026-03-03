using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

[TestFixture]
public class GetProjectTaskByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProjectTaskByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProjectTaskByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectTaskExists_ReturnsProjectTaskResponse()
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
            Priority = Common.Enums.Project.TaskPriority.High,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(task);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(taskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(taskId));
            Assert.That(result.Title, Is.EqualTo(task.Title));
        }
    }

    [Test]
    public async Task Handle_WhenProjectTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectTaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectTaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectTasks_ReturnsOnlyRequestedTask()
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
            Priority = Common.Enums.Project.TaskPriority.High,
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
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.AddRange(task1, task2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(task1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(task1Id));
            Assert.That(result.Title, Is.EqualTo(task1.Title));
        }
    }

    [Test]
    public async Task Handle_WithNullDescription_ReturnsCorrectResponse()
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
            Description = null,
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = null,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(task);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(taskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(taskId));
            Assert.That(result.Description, Is.Null);
        }
    }
}
