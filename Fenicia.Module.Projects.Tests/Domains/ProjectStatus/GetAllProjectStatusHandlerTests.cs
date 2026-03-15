using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class GetAllProjectStatusHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectStatusHandler handler;

    public GetAllProjectStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllProjectStatusHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectStatusQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectStatuses_ReturnsAllProjectStatuses()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var status1 = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        db.ProjectStatuses.AddRange(status1, status2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(status1.Id, result[0].Id);
        Assert.Equal(status2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var status = new ProjectStatusModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = $"{faker.Lorem.Word()} {i}",
                Color = faker.Internet.Color(),
                Order = i,
                IsFinal = i % 2 == 0
            };
            db.ProjectStatuses.Add(status);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery(2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var status = new ProjectStatusModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = $"{faker.Lorem.Word()} {i}",
                Color = faker.Internet.Color(),
                Order = i,
                IsFinal = false
            };
            db.ProjectStatuses.Add(status);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery(10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var status = new ProjectStatusModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = $"{faker.Lorem.Word()} {i}",
                Color = faker.Internet.Color(),
                Order = i,
                IsFinal = false
            };
            db.ProjectStatuses.Add(status);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}
