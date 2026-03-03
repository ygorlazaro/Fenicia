using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

[TestFixture]
public class AddProjectSubtaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectSubtaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectSubtaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectSubtaskAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            false,
            this.faker.Random.Number(0, 100),
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Title, Is.EqualTo(command.Title));
        }
    }

    [Test]
    public async Task Handle_VerifiesProjectSubtaskWasSaved()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            false,
            1,
            null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var subtask = await this.context.ProjectSubtasks
            .FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.That(subtask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(subtask.Title, Is.EqualTo(command.Title));
            Assert.That(subtask.IsCompleted, Is.EqualTo(command.IsCompleted));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjectSubtasks()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command1 = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            taskId,
            this.faker.Lorem.Sentence(5),
            false,
            1,
            null);

        var command2 = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            taskId,
            this.faker.Lorem.Sentence(5),
            true,
            2,
            DateTime.UtcNow);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var subtasks = await this.context.ProjectSubtasks.ToListAsync();
        Assert.That(subtasks, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithIsCompletedTrue_AddsProjectSubtaskSuccessfully()
    {
        // Arrange
        var completedAt = DateTime.UtcNow;
        var command = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            true,
            5,
            completedAt);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.IsCompleted, Is.True);
            Assert.That(result.CompletedAt, Is.EqualTo(completedAt));
        }
    }

    [Test]
    public async Task Handle_WithNullCompletedAt_AddsProjectSubtaskSuccessfully()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            false,
            1,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.CompletedAt, Is.Null);
        }
    }
}
