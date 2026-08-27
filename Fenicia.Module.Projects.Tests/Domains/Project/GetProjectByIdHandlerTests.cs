using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class GetProjectByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetProjectByIdHandler handler;

    public GetProjectByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ReturnsProjectResponse()
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

        var query = new GetProjectByIdQuery(projectId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Equal(project.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsNull()
    {

        var query = new GetProjectByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetProjectByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjects_ReturnsOnlyRequestedProject()
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
            Status = EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        db.Projects.AddRange(project1, project2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(project1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(project1Id, result.Id);
        Assert.Equal(project1.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ReturnsCorrectResponse()
    {

        var projectId = Guid.NewGuid();
        var project = new ProjectModel
        {
            Id = projectId,
            Title = faker.Lorem.Sentence(5),
            Description = null,
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(projectId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Null(result.Description);
    }
}
