using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

[TestFixture]
public class GetProjectByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProjectByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProjectByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectExists_ReturnsProjectResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Common.Data.Models.Project
        {
            Id = projectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(projectId));
            Assert.That(result.Title, Is.EqualTo(project.Title));
        }
    }

    [Test]
    public async Task Handle_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjects_ReturnsOnlyRequestedProject()
    {
        // Arrange
        var project1Id = Guid.NewGuid();
        var project2Id = Guid.NewGuid();

        var project1 = new Common.Data.Models.Project
        {
            Id = project1Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new Common.Data.Models.Project
        {
            Id = project2Id,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.AddRange(project1, project2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(project1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(project1Id));
            Assert.That(result.Title, Is.EqualTo(project1.Title));
        }
    }

    [Test]
    public async Task Handle_WithNullDescription_ReturnsCorrectResponse()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Common.Data.Models.Project
        {
            Id = projectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = null,
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(projectId));
            Assert.That(result.Description, Is.Null);
        }
    }
}
