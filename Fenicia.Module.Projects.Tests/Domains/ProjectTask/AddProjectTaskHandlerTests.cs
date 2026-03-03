using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

[TestFixture]
public class AddProjectTaskHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectTaskHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectTaskHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectTaskAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            this.faker.Random.Number(0, 100),
            this.faker.Random.Number(1, 10),
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

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
    public async Task Handle_VerifiesProjectTaskWasSaved()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            1,
            5,
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var task = await this.context.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == command.Id);

        Assert.That(task, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(task.Title, Is.EqualTo(command.Title));
            Assert.That(task.Description, Is.EqualTo(command.Description));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjectTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var command1 = new AddProjectTaskCommand(
            Guid.NewGuid(),
            projectId,
            statusId,
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "High",
            "Task",
            1,
            5,
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

        var command2 = new AddProjectTaskCommand(
            Guid.NewGuid(),
            projectId,
            statusId,
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Low",
            "Bug",
            2,
            3,
            null,
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var tasks = await this.context.ProjectTasks.ToListAsync();
        Assert.That(tasks, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithNullDescription_AddsProjectTaskSuccessfully()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            null,
            "Medium",
            "Task",
            1,
            null,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Description, Is.Null);
        }
    }

    [Test]
    public async Task Handle_WithNullDueDate_AddsProjectTaskSuccessfully()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
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
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.DueDate, Is.Null);
        }
    }
}
