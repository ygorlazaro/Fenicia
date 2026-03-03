using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

[TestFixture]
public class AddProjectTaskAssigneeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectTaskAssigneeHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectTaskAssigneeHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectTaskAssigneeAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Role, Is.EqualTo(command.Role));
        }
    }

    [Test]
    public async Task Handle_VerifiesProjectTaskAssigneeWasSaved()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            taskId,
            userId,
            "Owner",
            assignedAt);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var assignee = await this.context.ProjectTaskAssignees
            .FirstOrDefaultAsync(a => a.Id == command.Id);

        Assert.That(assignee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(assignee.TaskId, Is.EqualTo(taskId));
            Assert.That(assignee.UserId, Is.EqualTo(userId));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjectTaskAssignees()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command1 = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            taskId,
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow.AddDays(-5));

        var command2 = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            taskId,
            Guid.NewGuid(),
            "Contributor",
            DateTime.UtcNow.AddDays(-3));

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var assignees = await this.context.ProjectTaskAssignees.ToListAsync();
        Assert.That(assignees, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithMemberRole_AddsProjectTaskAssigneeSuccessfully()
    {
        // Arrange
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Contributor",
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Role, Is.EqualTo("Contributor"));
        }
    }

    [Test]
    public async Task Handle_WithPastAssignedDate_AddsProjectTaskAssigneeSuccessfully()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-30);
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            pastDate);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.AssignedAt, Is.EqualTo(pastDate));
        }
    }
}
