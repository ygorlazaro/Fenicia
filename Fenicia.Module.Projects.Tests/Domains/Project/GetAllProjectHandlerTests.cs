using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

[TestFixture]
public class GetAllProjectHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjects_ReturnsAllProjects()
    {
        // Arrange
        var project1 = new Common.Data.Models.Project
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = null,
            Owner = Guid.NewGuid()
        };

        var project2 = new Common.Data.Models.Project
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.AddRange(project1, project2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(project1.Id));
            Assert.That(result[1].Id, Is.EqualTo(project2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var project = new Common.Data.Models.Project
            {
                Id = Guid.NewGuid(),
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Status = Common.Enums.Project.ProjectStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Owner = Guid.NewGuid()
            };
            this.context.Projects.Add(project);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            var project = new Common.Data.Models.Project
            {
                Id = Guid.NewGuid(),
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Status = Common.Enums.Project.ProjectStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Owner = Guid.NewGuid()
            };
            this.context.Projects.Add(project);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var project = new Common.Data.Models.Project
            {
                Id = Guid.NewGuid(),
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Status = Common.Enums.Project.ProjectStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Owner = Guid.NewGuid()
            };
            this.context.Projects.Add(project);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
