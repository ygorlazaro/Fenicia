using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Position.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

[TestFixture]
public class GetPositionByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options);
        this.handler = new GetPositionByIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context = null!;
    private GetPositionByIdHandler handler = null!;

    [Test]
    public async Task Handle_WhenPositionExists_ReturnsPositionResponse()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new BasicPosition
        {
            Id = positionId,
            Name = "Developer"
        };

        this.context.BasicPositions.Add(position);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetPositionByIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(positionId));
            Assert.That(result.Name, Is.EqualTo("Developer"));
        }
    }

    [Test]
    public async Task Handle_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetPositionByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetPositionByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultiplePositions_ReturnsOnlyRequestedPosition()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new BasicPosition { Id = position1Id, Name = "Developer" };
        var position2 = new BasicPosition { Id = position2Id, Name = "Designer" };

        this.context.BasicPositions.AddRange(position1, position2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetPositionByIdQuery(position1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(position1Id));
            Assert.That(result.Name, Is.EqualTo("Developer"));
        }
        Assert.That(result.Name, Is.Not.EqualTo("Designer"));
    }
}
