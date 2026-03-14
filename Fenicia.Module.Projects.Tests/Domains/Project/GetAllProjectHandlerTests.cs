using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class GetAllProjectHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectHandler handler;

    public GetAllProjectHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetAllProjectHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjects_ReturnsAllProjects()
    {
        // Arrange
        var project1 = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.AddRange(project1, project2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(project1.Id, result[0].Id);
        Assert.Equal(project2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var project = new ProjectModel
            {
                Id = Guid.NewGuid(),
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Status = EnumProjectStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Owner = Guid.NewGuid()
            };
            this.db.Projects.Add(project);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            var project = new ProjectModel
            {
                Id = Guid.NewGuid(),
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Status = EnumProjectStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Owner = Guid.NewGuid()
            };
            this.db.Projects.Add(project);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var project = new ProjectModel
            {
                Id = Guid.NewGuid(),
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Status = EnumProjectStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Owner = Guid.NewGuid()
            };
            this.db.Projects.Add(project);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}