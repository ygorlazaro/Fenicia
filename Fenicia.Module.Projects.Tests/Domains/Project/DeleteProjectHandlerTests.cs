using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.Project.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class DeleteProjectHandlerTests : IDisposable
{
    public DeleteProjectHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new DeleteProjectHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly DeleteProjectHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectExists_SetsDeletedDate()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new ProjectModel
        {
            Id = projectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.Add(project);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(projectId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedProject = await this.db.Projects.FindAsync([
                projectId
            ],
            CancellationToken.None);
        Assert.NotNull(deletedProject);
        Assert.NotNull(deletedProject.Deleted);
        Assert.InRange(deletedProject.Deleted.Value,
            beforeDelete.AddSeconds(-1),
            DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var projects = await this.db.Projects.ToListAsync();
        Assert.Empty(projects);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var projects = await this.db.Projects.ToListAsync();
        Assert.Empty(projects);
    }

    [Fact]
    public async Task Handle_WithMultipleProjects_OnlyDeletesSpecified()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();

        var project1 = new ProjectModel
        {
            Id = project1Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = project2Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.EnumProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.AddRange(project1,
            project2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project1Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedProject = await this.db.Projects.FindAsync([
                project1Id
            ],
            CancellationToken.None);
        var notDeletedProject = await this.db.Projects.FindAsync([
                project2Id
            ],
            CancellationToken.None);

        Assert.NotNull(deletedProject);
        Assert.NotNull(deletedProject.Deleted);
        Assert.NotNull(notDeletedProject);
        Assert.Null(notDeletedProject.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjects_DeletesCorrectProject()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();
        var project3Id = Guid.NewGuid();

        var project1 = new ProjectModel
        {
            Id = project1Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = project2Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.EnumProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project3 = new ProjectModel
        {
            Id = project3Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.AddRange(project1,
            project2,
            project3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project2Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var project1InDb = await this.db.Projects.FindAsync([
                project1Id
            ],
            CancellationToken.None);
        var deletedProject = await this.db.Projects.FindAsync([
                project2Id
            ],
            CancellationToken.None);
        var project3InDb = await this.db.Projects.FindAsync([
                project3Id
            ],
            CancellationToken.None);

        Assert.NotNull(project1InDb);
        Assert.NotNull(deletedProject);
        Assert.NotNull(project3InDb);
        Assert.Null(project1InDb.Deleted);
        Assert.NotNull(deletedProject.Deleted);
        Assert.Null(project3InDb.Deleted);
    }
}
