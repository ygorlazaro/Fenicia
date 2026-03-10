using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class AddProjectHandlerTests : IDisposable
{
    public AddProjectHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new AddProjectHandler(this.context);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly AddProjectHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(6),
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Title, result.Title);
    }

    [Fact]
    public async Task Handle_VerifiesProjectWasSaved()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(6),
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var project = await this.context.Projects
            .FirstOrDefaultAsync(p => p.Id == command.Id);

        Assert.NotNull(project);
        Assert.Equal(command.Title, project.Title);
        Assert.Equal(command.Description, project.Description);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjects()
    {
        // Arrange
        var command1 = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        var command2 = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Draft",
            DateTime.UtcNow.AddDays(-5),
            null,
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var projects = await this.context.Projects.ToListAsync();
        Assert.Equal(2, projects.Count);
    }

    [Fact]
    public async Task Handle_WithNullDescription_AddsProjectSuccessfully()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            null,
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task Handle_WithNullEndDate_AddsProjectSuccessfully()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Null(result.EndDate);
    }
}
