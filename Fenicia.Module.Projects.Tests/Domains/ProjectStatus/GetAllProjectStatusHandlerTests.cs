using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

[TestFixture]
public class GetAllProjectStatusHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectStatusHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectStatusHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectStatusQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjectStatuses_ReturnsAllProjectStatuses()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var status1 = new Common.Data.Models.ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var status2 = new Common.Data.Models.ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        this.context.ProjectStatuses.AddRange(status1, status2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(status1.Id));
            Assert.That(result[1].Id, Is.EqualTo(status2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var status = new Common.Data.Models.ProjectStatusModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = $"{this.faker.Lorem.Word()} {i}",
                Color = this.faker.Internet.Color(),
                Order = i,
                IsFinal = i % 2 == 0
            };
            this.context.ProjectStatuses.Add(status);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery(2);

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
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var status = new Common.Data.Models.ProjectStatusModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = $"{this.faker.Lorem.Word()} {i}",
                Color = this.faker.Internet.Color(),
                Order = i,
                IsFinal = false
            };
            this.context.ProjectStatuses.Add(status);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery(10);

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
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var status = new Common.Data.Models.ProjectStatusModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = $"{this.faker.Lorem.Word()} {i}",
                Color = this.faker.Internet.Color(),
                Order = i,
                IsFinal = false
            };
            this.context.ProjectStatuses.Add(status);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectStatusQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
