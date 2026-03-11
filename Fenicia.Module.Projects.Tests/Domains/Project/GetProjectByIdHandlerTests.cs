using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.Project.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class GetProjectByIdHandlerTests : IDisposable
{
    public GetProjectByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetProjectByIdHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly GetProjectByIdHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectExists_ReturnsProjectResponse()
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

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Equal(project.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjects_ReturnsOnlyRequestedProject()
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
            Status = Common.Enums.Project.EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.AddRange(project1, project2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(project1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(project1Id, result.Id);
        Assert.Equal(project1.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ReturnsCorrectResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new ProjectModel
        {
            Id = projectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = null,
            Status = Common.Enums.Project.EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.Add(project);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Null(result.Description);
    }
}
