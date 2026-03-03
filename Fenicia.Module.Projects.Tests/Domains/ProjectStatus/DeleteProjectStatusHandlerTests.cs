using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectStatus.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

[TestFixture]
public class DeleteProjectStatusHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectStatusHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectStatusHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectStatusExists_SetsDeletedDate()
    {
        // Arrange
        var statusId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var status = new Common.Data.Models.ProjectStatus
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
        Assert.That(deletedStatus, Is.Not.Null);
        Assert.That(deletedStatus.Deleted, Is.Not.Null);
        Assert.That(deletedStatus.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedStatus.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectStatusDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectStatusCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var statuses = await this.context.ProjectStatuses.ToListAsync();
        Assert.That(statuses, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectStatusCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var statuses = await this.context.ProjectStatuses.ToListAsync();
        Assert.That(statuses, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjectStatuses_OnlyDeletesSpecified()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new Common.Data.Models.ProjectStatus
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new Common.Data.Models.ProjectStatus
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

        Assert.That(deletedStatus, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedStatus.Deleted, Is.Not.Null);
            Assert.That(notDeletedStatus, Is.Not.Null);
        }
        Assert.That(notDeletedStatus?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectStatuses_DeletesCorrectProjectStatus()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var status3Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new Common.Data.Models.ProjectStatus
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new Common.Data.Models.ProjectStatus
        {
            Id = status2Id,
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        var status3 = new Common.Data.Models.ProjectStatus
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

        Assert.That(status1InDb, Is.Not.Null);
        Assert.That(deletedStatus, Is.Not.Null);
        Assert.That(status3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(status1InDb.Deleted, Is.Null);
            Assert.That(deletedStatus.Deleted, Is.Not.Null);
            Assert.That(status3InDb.Deleted, Is.Null);
        }
    }
}
