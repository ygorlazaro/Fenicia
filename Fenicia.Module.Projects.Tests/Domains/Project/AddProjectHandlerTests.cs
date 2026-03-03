using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

[TestFixture]
public class AddProjectHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(6),
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
    public async Task Handle_VerifiesProjectWasSaved()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(6),
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var project = await this.context.Projects
            .FirstOrDefaultAsync(p => p.Id == command.Id);

        Assert.That(project, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(project.Title, Is.EqualTo(command.Title));
            Assert.That(project.Description, Is.EqualTo(command.Description));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjects()
    {
        // Arrange
        var command1 = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        var command2 = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Draft",
            DateTime.UtcNow.AddDays(-5),
            null,
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var projects = await this.context.Projects.ToListAsync();
        Assert.That(projects, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithNullDescription_AddsProjectSuccessfully()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            null,
            "Active",
            DateTime.UtcNow,
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
    public async Task Handle_WithNullEndDate_AddsProjectSuccessfully()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.EndDate, Is.Null);
        }
    }
}
