using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectStatus.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

[TestFixture]
public class UpdateProjectStatusHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectStatusHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectStatusHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectStatusExists_UpdatesProjectStatusAndReturnsResponse()
    {
        // Arrange
        var statusId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var status = new Common.Data.Models.ProjectStatus
        {
            Id = statusId,
            ProjectId = projectId,
            Name = "Old Status",
            Color = "#FFFFFF",
            Order = 1,
            IsFinal = false
        };

        this.context.ProjectStatuses.Add(status);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(
            statusId,
            projectId,
            "New Status",
            "#000000",
            5,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(statusId));
            Assert.That(result.Name, Is.EqualTo("New Status"));
        }
    }

    [Test]
    public async Task Handle_WhenProjectStatusDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Status",
            "#000000",
            5,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Status",
            "#000000",
            5,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectStatus()
    {
        // Arrange
        var status1Id = Guid.NewGuid();
        var status2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var status1 = new Common.Data.Models.ProjectStatus
        {
            Id = status1Id,
            ProjectId = projectId,
            Name = "Status 1",
            Color = "#FF0000",
            Order = 1,
            IsFinal = false
        };

        var status2 = new Common.Data.Models.ProjectStatus
        {
            Id = status2Id,
            ProjectId = projectId,
            Name = "Status 2",
            Color = "#00FF00",
            Order = 2,
            IsFinal = true
        };

        this.context.ProjectStatuses.AddRange(status1, status2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(
            status1Id,
            projectId,
            "Updated Status 1",
            "#0000FF",
            10,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(status1Id));
            Assert.That(result.Name, Is.EqualTo("Updated Status 1"));
        }

        var updatedStatus1 = await this.context.ProjectStatuses.FindAsync([status1Id], CancellationToken.None);
        var status2InDb = await this.context.ProjectStatuses.FindAsync([status2Id], CancellationToken.None);

        Assert.That(updatedStatus1, Is.Not.Null);
        Assert.That(status2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedStatus1.Name, Is.EqualTo("Updated Status 1"));
            Assert.That(status2InDb.Name, Is.EqualTo("Status 2"));
        }
    }

    [Test]
    public async Task Handle_WithIsFinalChange_UpdatesProjectStatusSuccessfully()
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

        var command = new UpdateProjectStatusCommand(
            statusId,
            projectId,
            "Updated Status",
            "#123456",
            3,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(statusId));
            Assert.That(result.IsFinal, Is.True);
        }
    }
}
