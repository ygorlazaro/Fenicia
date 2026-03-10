using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectStatus.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class DeleteProjectStatusHandlerTests : IDisposable
{
    public DeleteProjectStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new DeleteProjectStatusHandler(this.context);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly DeleteProjectStatusHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectStatusExists_SetsDeletedDate()
    {
        // Arrange
        var statusId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var status = new ProjectStatusModel
        {
            Id = statusId,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        this.context.ProjectStatuses.Add(status);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectStatusCommand(statusId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedStatus = await this.context.ProjectStatuses.FindAsync([statusId], CancellationToken.None);
        Assert.NotNull(deletedStatus);
        Assert.NotNull(deletedStatus.Deleted);
        Assert.InRange(deletedStatus.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectStatusDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectStatusCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var statuses = await this.context.ProjectStatuses.ToListAsync();
        Assert.Empty(statuses);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectStatusCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var statuses = await this.context.ProjectStatuses.ToListAsync();
        Assert.Empty(statuses);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectStatuses_OnlyDeletesSpecified()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new ProjectStatusModel
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new ProjectStatusModel
        {
            Id = status2Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        this.context.ProjectStatuses.AddRange(status1, status2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectStatusCommand(status1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedStatus = await this.context.ProjectStatuses.FindAsync([status1Id], CancellationToken.None);
        var notDeletedStatus = await this.context.ProjectStatuses.FindAsync([status2Id], CancellationToken.None);

        Assert.NotNull(deletedStatus);
        Assert.NotNull(deletedStatus.Deleted);
        Assert.NotNull(notDeletedStatus);
        Assert.Null(notDeletedStatus.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectStatuses_DeletesCorrectProjectStatus()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var status3Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new ProjectStatusModel
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new ProjectStatusModel
        {
            Id = status2Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        var status3 = new ProjectStatusModel
        {
            Id = status3Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 3,
            IsFinal = false
        };

        this.context.ProjectStatuses.AddRange(status1, status2, status3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectStatusCommand(status2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var status1InDb = await this.context.ProjectStatuses.FindAsync([status1Id], CancellationToken.None);
        var deletedStatus = await this.context.ProjectStatuses.FindAsync([status2Id], CancellationToken.None);
        var status3InDb = await this.context.ProjectStatuses.FindAsync([status3Id], CancellationToken.None);

        Assert.NotNull(status1InDb);
        Assert.NotNull(deletedStatus);
        Assert.NotNull(status3InDb);
        Assert.Null(status1InDb.Deleted);
        Assert.NotNull(deletedStatus.Deleted);
        Assert.Null(status3InDb.Deleted);
    }
}
