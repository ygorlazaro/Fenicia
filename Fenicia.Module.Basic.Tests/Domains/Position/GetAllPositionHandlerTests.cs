using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

[TestFixture]
public class GetAllPositionHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetAllPositionHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetAllPositionHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllPositionQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithPositions_ReturnsAllPositions()
    {
        // Arrange
        var position1 = new PositionModel { Id = Guid.NewGuid(), Name = "Developer" };
        var position2 = new PositionModel { Id = Guid.NewGuid(), Name = "Designer" };

        this.context.Positions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Any(p => p.Id == position1.Id));
            Assert.That(result.Any(p => p.Id == position2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var position = new PositionModel
            {
                Id = Guid.NewGuid(),
                Name = $"{this.faker.Commerce.Department()} {i}"
            };
            this.context.Positions.Add(position);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery(2);

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
            var position = new PositionModel
            {
                Id = Guid.NewGuid(),
                Name = $"{this.faker.Commerce.Department()} {i}"
            };
            this.context.Positions.Add(position);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery(10);

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
            var position = new PositionModel
            {
                Id = Guid.NewGuid(),
                Name = $"{this.faker.Commerce.Department()} {i}"
            };
            this.context.Positions.Add(position);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}
