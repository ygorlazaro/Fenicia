using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

[TestFixture]
public class GetProjectSubtaskByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProjectSubtaskByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProjectSubtaskByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectSubtaskExists_ReturnsProjectSubtaskResponse()
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

        var query = new GetProjectSubtaskByIdQuery(subtaskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(subtaskId));
            Assert.That(result.Title, Is.EqualTo(subtask.Title));
        }
    }

    [Test]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectSubtaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectSubtaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectSubtasks_ReturnsOnlyRequestedSubtask()
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

        var query = new GetProjectSubtaskByIdQuery(subtask1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(subtask1Id));
            Assert.That(result.Title, Is.EqualTo(subtask1.Title));
        }
    }

    [Test]
    public async Task Handle_WithCompletedSubtask_ReturnsCorrectResponse()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var completedAt = DateTime.UtcNow.AddDays(-5);
        var subtask = new Common.Data.Models.ProjectSubtask
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 1,
            CompletedAt = completedAt
        };

        this.context.ProjectSubtasks.Add(subtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtaskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

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
