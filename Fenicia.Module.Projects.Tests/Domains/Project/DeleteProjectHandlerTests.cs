using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class DeleteProjectHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteProjectHandler handler;

    public DeleteProjectHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteProjectHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_SetsDeletedDate()
    {

        var projectId = Guid.NewGuid();
        var project = new ProjectModel
        {
            Id = projectId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(projectId);
        var beforeDelete = DateTime.UtcNow;

        await handler.Handle(command, CancellationToken.None);

        var deletedProject = await db.Projects.FindAsync([projectId], CancellationToken.None);
        Assert.NotNull(deletedProject);
        Assert.NotNull(deletedProject.Deleted);
        Assert.InRange(deletedProject.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_DoesNothing()
    {

        var command = new DeleteProjectCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var projects = await db.Projects.ToListAsync();
        Assert.Empty(projects);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {

        var command = new DeleteProjectCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var projects = await db.Projects.ToListAsync();
        Assert.Empty(projects);
    }

    [Fact]
    public async Task Handle_WithMultipleProjects_OnlyDeletesSpecified()
    {

        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();

        var project1 = new ProjectModel
        {
            Id = project1Id,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = project2Id,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        db.Projects.AddRange(project1, project2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project1Id);

        await handler.Handle(command, CancellationToken.None);

        var deletedProject = await db.Projects.FindAsync([project1Id], CancellationToken.None);
        var notDeletedProject = await db.Projects.FindAsync([project2Id], CancellationToken.None);

        Assert.NotNull(deletedProject);
        Assert.NotNull(deletedProject.Deleted);
        Assert.NotNull(notDeletedProject);
        Assert.Null(notDeletedProject.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjects_DeletesCorrectProject()
    {

        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();
        var project3Id = Guid.NewGuid();

        var project1 = new ProjectModel
        {
            Id = project1Id,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = project2Id,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project3 = new ProjectModel
        {
            Id = project3Id,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        db.Projects.AddRange(project1, project2, project3);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommand(project2Id);

        await handler.Handle(command, CancellationToken.None);

        var project1InDb = await db.Projects.FindAsync([project1Id], CancellationToken.None);
        var deletedProject = await db.Projects.FindAsync([project2Id], CancellationToken.None);
        var project3InDb = await db.Projects.FindAsync([project3Id], CancellationToken.None);

        Assert.NotNull(project1InDb);
        Assert.NotNull(deletedProject);
        Assert.NotNull(project3InDb);
        Assert.Null(project1InDb.Deleted);
        Assert.NotNull(deletedProject.Deleted);
        Assert.Null(project3InDb.Deleted);
    }
}
