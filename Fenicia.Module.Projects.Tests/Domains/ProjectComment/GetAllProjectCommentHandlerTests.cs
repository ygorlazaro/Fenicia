using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

[TestFixture]
public class GetAllProjectCommentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectCommentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectCommentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectCommentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjectComments_ReturnsAllProjectComments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment1 = new Common.Data.Models.ProjectComment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        var comment2 = new Common.Data.Models.ProjectComment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.AddRange(comment1, comment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(comment1.Id));
            Assert.That(result[1].Id, Is.EqualTo(comment2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var comment = new Common.Data.Models.ProjectComment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = $"{this.faker.Lorem.Paragraph()} {i}"
            };
            this.context.ProjectComments.Add(comment);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery(2);

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
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var comment = new Common.Data.Models.ProjectComment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = $"{this.faker.Lorem.Paragraph()} {i}"
            };
            this.context.ProjectComments.Add(comment);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery(10);

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
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var comment = new Common.Data.Models.ProjectComment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = $"{this.faker.Lorem.Paragraph()} {i}"
            };
            this.context.ProjectComments.Add(comment);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
