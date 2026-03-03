using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectStatus.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

[TestFixture]
public class AddProjectStatusHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectStatusHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectStatusHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectStatusAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            this.faker.Random.Number(0, 100),
            this.faker.Random.Bool());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Name, Is.EqualTo(command.Name));
        }
    }

    [Test]
    public async Task Handle_VerifiesProjectStatusWasSaved()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            this.faker.Random.Number(0, 100),
            this.faker.Random.Bool());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var status = await this.context.ProjectStatuses
            .FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.That(status, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(status.Name, Is.EqualTo(command.Name));
            Assert.That(status.Color, Is.EqualTo(command.Color));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjectStatuses()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command1 = new AddProjectStatusCommand(
            Guid.NewGuid(),
            projectId,
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            1,
            false);

        var command2 = new AddProjectStatusCommand(
            Guid.NewGuid(),
            projectId,
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            2,
            true);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var statuses = await this.context.ProjectStatuses.ToListAsync();
        Assert.That(statuses, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithIsFinalTrue_AddsProjectStatusSuccessfully()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            10,
            true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.IsFinal, Is.True);
        }
    }

    [Test]
    public async Task Handle_WithOrderZero_AddsProjectStatusSuccessfully()
    {
        // Arrange
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            0,
            false);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Order, Is.EqualTo(0));
        }
    }
}
