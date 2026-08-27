using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class DeleteProjectStatusHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteProjectStatusHandler handler;

    public DeleteProjectStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteProjectStatusHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectStatusExists_SetsDeletedDate()
    {

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

        var command = new DeleteProjectStatusCommand(statusId);
        var beforeDelete = DateTime.UtcNow;

        await handler.Handle(command, CancellationToken.None);

        var deletedStatus = await db.ProjectStatuses.FindAsync([statusId], CancellationToken.None);
        Assert.NotNull(deletedStatus);
        Assert.NotNull(deletedStatus.Deleted);
        Assert.InRange(deletedStatus.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectStatusDoesNotExist_DoesNothing()
    {

        var command = new DeleteProjectStatusCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var statuses = await db.ProjectStatuses.ToListAsync();
        Assert.Empty(statuses);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {

        var command = new DeleteProjectStatusCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var statuses = await db.ProjectStatuses.ToListAsync();
        Assert.Empty(statuses);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectStatuses_OnlyDeletesSpecified()
    {

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

        var command = new DeleteProjectStatusCommand(status1Id);

        await handler.Handle(command, CancellationToken.None);

        var deletedStatus = await db.ProjectStatuses.FindAsync([status1Id], CancellationToken.None);
        var notDeletedStatus = await db.ProjectStatuses.FindAsync([status2Id], CancellationToken.None);

        Assert.NotNull(deletedStatus);
        Assert.NotNull(deletedStatus.Deleted);
        Assert.NotNull(notDeletedStatus);
        Assert.Null(notDeletedStatus.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectStatuses_DeletesCorrectProjectStatus()
    {

        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var status3Id = Guid.NewGuid();
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

        var status3 = new ProjectStatusModel
        {
            Id = status3Id,
            ProjectId = projectId,
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 3,
            IsFinal = false
        };

        db.ProjectStatuses.AddRange(status1, status2, status3);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectStatusCommand(status2Id);

        await handler.Handle(command, CancellationToken.None);

        var status1InDb = await db.ProjectStatuses.FindAsync([status1Id], CancellationToken.None);
        var deletedStatus = await db.ProjectStatuses.FindAsync([status2Id], CancellationToken.None);
        var status3InDb = await db.ProjectStatuses.FindAsync([status3Id], CancellationToken.None);

        Assert.NotNull(status1InDb);
        Assert.NotNull(deletedStatus);
        Assert.NotNull(status3InDb);
        Assert.Null(status1InDb.Deleted);
        Assert.NotNull(deletedStatus.Deleted);
        Assert.Null(status3InDb.Deleted);
    }
}
