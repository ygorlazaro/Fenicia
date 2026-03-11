using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.Project.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class UpdateProjectHandlerTests : IDisposable
{
    public UpdateProjectHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new UpdateProjectHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly UpdateProjectHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectExists_UpdatesProjectAndReturnsResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new ProjectModel
        {
            Id = projectId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Common.Enums.Project.EnumProjectStatus.Draft,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.Add(project);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Equal("New Title", result.Title);
    }

    [Fact]
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
        Assert.Null(result);
    }

    [Fact]
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
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProject()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();

        var project1 = new ProjectModel
        {
            Id = project1Id,
            Title = "Project 1 Title",
            Description = "Project 1 Description",
            Status = Common.Enums.Project.EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = project2Id,
            Title = "Project 2 Title",
            Description = "Project 2 Description",
            Status = Common.Enums.Project.EnumProjectStatus.Draft,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.AddRange(project1, project2);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        Assert.NotNull(result);
        Assert.Equal(project1Id, result.Id);
        Assert.Equal("Updated Project 1 Title", result.Title);

        var updatedProject1 = await this.db.Projects.FindAsync([project1Id], CancellationToken.None);
        var project2InDb = await this.db.Projects.FindAsync([project2Id], CancellationToken.None);

        Assert.NotNull(updatedProject1);
        Assert.NotNull(project2InDb);
        Assert.Equal("Updated Project 1 Title", updatedProject1.Title);
        Assert.Equal("Project 2 Title", project2InDb.Title);
    }

    [Fact]
    public async Task Handle_WithNullDescription_UpdatesProjectSuccessfully()
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
        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Null(result.Description);
    }
}
