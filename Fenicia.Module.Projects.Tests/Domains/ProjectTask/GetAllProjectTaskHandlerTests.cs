using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

[TestFixture]
public class GetAllProjectTaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectTaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectTaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectTaskQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjectTasks_ReturnsAllProjectTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task1 = new Common.Data.Models.ProjectTask
        {
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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

        var query = new GetAllProjectTaskQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(task1.Id));
            Assert.That(result[1].Id, Is.EqualTo(task2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var task = new Common.Data.Models.ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Priority = Common.Enums.Project.TaskPriority.Medium,
                Type = Common.Enums.Project.TaskType.Task,
                Order = i,
                EstimatePoints = this.faker.Random.Number(1, 10),
                DueDate = DateTime.UtcNow.AddDays(i),
                CreatedBy = Guid.NewGuid()
            };
            this.context.ProjectTasks.Add(task);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery(2);

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
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var task = new Common.Data.Models.ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Priority = Common.Enums.Project.TaskPriority.Medium,
                Type = Common.Enums.Project.TaskType.Task,
                Order = i,
                EstimatePoints = 5,
                DueDate = null,
                CreatedBy = Guid.NewGuid()
            };
            this.context.ProjectTasks.Add(task);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery(10);

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
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var task = new Common.Data.Models.ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Priority = Common.Enums.Project.TaskPriority.Medium,
                Type = Common.Enums.Project.TaskType.Task,
                Order = i,
                EstimatePoints = 5,
                DueDate = null,
                CreatedBy = Guid.NewGuid()
            };
            this.context.ProjectTasks.Add(task);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
