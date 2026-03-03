using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

[TestFixture]
public class GetAllProjectSubtaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectSubtaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectSubtaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectSubtaskQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjectSubtasks_ReturnsAllProjectSubtasks()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var subtask1 = new Common.Data.Models.ProjectSubtask
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new Common.Data.Models.ProjectSubtask
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        this.context.ProjectSubtasks.AddRange(subtask1, subtask2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(subtask1.Id));
            Assert.That(result[1].Id, Is.EqualTo(subtask2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var subtask = new Common.Data.Models.ProjectSubtask
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                IsCompleted = i % 2 == 0,
                Order = i,
                CompletedAt = i % 2 == 0 ? DateTime.UtcNow.AddDays(-i) : null
            };
            this.context.ProjectSubtasks.Add(subtask);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery(2);

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
            var subtask = new Common.Data.Models.ProjectSubtask
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                IsCompleted = false,
                Order = i,
                CompletedAt = null
            };
            this.context.ProjectSubtasks.Add(subtask);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery(10);

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
            var subtask = new Common.Data.Models.ProjectSubtask
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                IsCompleted = false,
                Order = i,
                CompletedAt = null
            };
            this.context.ProjectSubtasks.Add(subtask);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
