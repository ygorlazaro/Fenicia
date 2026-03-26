using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class GetProjectStatusByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetProjectStatusByIdHandler handler;

    public GetProjectStatusByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectStatusByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectStatusExists_ReturnsProjectStatusResponse()
    {
        // Arrange
        var statusId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var status = new ProjectStatusModel
        {
            Id = statusId,
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        db.ProjectStatuses.Add(status);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectStatusByIdQuery(statusId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusId, result.Id);
        Assert.Equal(status.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WhenProjectStatusDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectStatusByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectStatusByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectStatuses_ReturnsOnlyRequestedStatus()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new ProjectStatusModel
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new ProjectStatusModel
        {
            Id = status2Id,
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        db.ProjectStatuses.AddRange(status1, status2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectStatusByIdQuery(status1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(status1Id, result.Id);
        Assert.Equal(status1.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithIsFinalTrue_ReturnsCorrectResponse()
    {
        // Arrange
        var statusId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var status = new ProjectStatusModel
        {
            Id = statusId,
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 5,
            IsFinal = true
        };

        db.ProjectStatuses.Add(status);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectStatusByIdQuery(statusId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusId, result.Id);
        Assert.True(result.IsFinal);
    }
}
