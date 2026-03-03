using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

[TestFixture]
public class DeleteProjectHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectExists_SetsDeletedDate()
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

        var command = new DeleteProjectCommand(projectId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedProject = await this.context.Projects.FindAsync([projectId], CancellationToken.None);
        Assert.That(deletedProject, Is.Not.Null);
        Assert.That(deletedProject.Deleted, Is.Not.Null);
        Assert.That(deletedProject.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedProject.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var projects = await this.context.Projects.ToListAsync();
        Assert.That(projects, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var projects = await this.context.Projects.ToListAsync();
        Assert.That(projects, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjects_OnlyDeletesSpecified()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();

        var project1 = new Common.Data.Models.ProjectModel
        {
            Id = project1Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status =Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new Common.Data.Models.ProjectModel
        {
            Id = project2Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.AddRange(project1, project2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedProject = await this.context.Projects.FindAsync([project1Id], CancellationToken.None);
        var notDeletedProject = await this.context.Projects.FindAsync([project2Id], CancellationToken.None);

        Assert.That(deletedProject, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedProject.Deleted, Is.Not.Null);
            Assert.That(notDeletedProject, Is.Not.Null);
        }
        Assert.That(notDeletedProject?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjects_DeletesCorrectProject()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();
        var project3Id = Guid.NewGuid();

        var project1 = new Common.Data.Models.ProjectModel
        {
            Id = project1Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status =Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new Common.Data.Models.ProjectModel
        {
            Id = project2Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project3 = new Common.Data.Models.ProjectModel
        {
            Id = project3Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Completed,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.AddRange(project1, project2, project3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var project1InDb = await this.context.Projects.FindAsync([project1Id], CancellationToken.None);
        var deletedProject = await this.context.Projects.FindAsync([project2Id], CancellationToken.None);
        var project3InDb = await this.context.Projects.FindAsync([project3Id], CancellationToken.None);

        Assert.That(project1InDb, Is.Not.Null);
        Assert.That(deletedProject, Is.Not.Null);
        Assert.That(project3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(project1InDb.Deleted, Is.Null);
            Assert.That(deletedProject.Deleted, Is.Not.Null);
            Assert.That(project3InDb.Deleted, Is.Null);
        }
    }
}
