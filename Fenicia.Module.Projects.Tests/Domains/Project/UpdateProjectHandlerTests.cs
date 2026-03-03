using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

[TestFixture]
public class UpdateProjectHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectExists_UpdatesProjectAndReturnsResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Common.Data.Models.ProjectModel
        {
            Id = projectId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Common.Enums.Project.ProjectStatus.Draft,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(
            projectId,
            "New Title",
            "New Description",
            "Completed",
            DateTime.UtcNow.AddDays(-20),
            DateTime.UtcNow,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(projectId));
            Assert.That(result.Title, Is.EqualTo("New Title"));
        }
    }

    [Test]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommand(
            Guid.NewGuid(),
            "New Title",
            "New Description",
            "Active",
            DateTime.UtcNow,
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
        var command = new UpdateProjectCommand(
            Guid.NewGuid(),
            "New Title",
            "New Description",
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProject()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();

        var project1 = new Common.Data.Models.ProjectModel
        {
            Id = project1Id,
            Title = "Project 1 Title",
            Description = "Project 1 Description",
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new Common.Data.Models.ProjectModel
        {
            Id = project2Id,
            Title = "Project 2 Title",
            Description = "Project 2 Description",
            Status = Common.Enums.Project.ProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.AddRange(project1, project2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(
            project1Id,
            "Updated Project 1 Title",
            "Updated Project 1 Description",
            "Completed",
            project1.StartDate,
            DateTime.UtcNow,
            project1.Owner);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(project1Id));
            Assert.That(result.Title, Is.EqualTo("Updated Project 1 Title"));
        }

        var updatedProject1 = await this.context.Projects.FindAsync([project1Id], CancellationToken.None);
        var project2InDb = await this.context.Projects.FindAsync([project2Id], CancellationToken.None);

        Assert.That(updatedProject1, Is.Not.Null);
        Assert.That(project2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedProject1.Title, Is.EqualTo("Updated Project 1 Title"));
            Assert.That(project2InDb.Title, Is.EqualTo("Project 2 Title"));
        }
    }

    [Test]
    public async Task Handle_WithNullDescription_UpdatesProjectSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Common.Data.Models.ProjectModel
        {
            Id = projectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(
            projectId,
            "Updated Title",
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
            Assert.That(result.Id, Is.EqualTo(projectId));
            Assert.That(result.Description, Is.Null);
        }
    }
}
