using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class UpdateProjectStatusHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateProjectStatusHandler handler;

    public UpdateProjectStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateProjectStatusHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectStatusExists_UpdatesProjectStatusAndReturnsResponse()
    {
        // Arrange
        var statusId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var status = new ProjectStatusModel
        {
            Id = statusId,
            ProjectId = projectId,
            Name = "Old Status",
            Color = "#FFFFFF",
            Order = 1,
            IsFinal = false
        };

        db.ProjectStatuses.Add(status);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(statusId, projectId, "New Status", "#000000", 5, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusId, result.Id);
        Assert.Equal("New Status", result.Name);
    }

    [Fact]
    public async Task Handle_WhenProjectStatusDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "New Status", "#000000", 5, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "New Status", "#000000", 5, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectStatus()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new ProjectStatusModel
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = "Status 1",
            Color = "#FF0000",
            Order = 1,
            IsFinal = false
        };

        var status2 = new ProjectStatusModel
        {
            Id = status2Id,
            ProjectId = projectId,
            Name = "Status 2",
            Color = "#00FF00",
            Order = 2,
            IsFinal = true
        };

        db.ProjectStatuses.AddRange(status1, status2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(status1Id, projectId, "Updated Status 1", "#0000FF", 10, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(status1Id, result.Id);
        Assert.Equal("Updated Status 1", result.Name);

        var updatedStatus1 = await db.ProjectStatuses.FindAsync([status1Id], CancellationToken.None);
        var status2InDb = await db.ProjectStatuses.FindAsync([status2Id], CancellationToken.None);

        Assert.NotNull(updatedStatus1);
        Assert.NotNull(status2InDb);
        Assert.Equal("Updated Status 1", updatedStatus1.Name);
        Assert.Equal("Status 2", status2InDb.Name);
    }

    [Fact]
    public async Task Handle_WithIsFinalChange_UpdatesProjectStatusSuccessfully()
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

        var command = new UpdateProjectStatusCommand(statusId, projectId, "Updated Status", "#123456", 3, true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusId, result.Id);
        Assert.True(result.IsFinal);
    }
}
