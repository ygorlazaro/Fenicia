using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position.Handlers;
using Fenicia.Module.Basic.Domains.Position.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class GetPositionByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetPositionByIdHandler handler;

    public GetPositionByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetPositionByIdHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData("Developer")]
    [InlineData("Designer")]
    [InlineData("Manager")]
    [InlineData("Analyst")]
    public async Task Handle_WhenPositionExists_ReturnsPositionResponse(string positionName)
    {
        // Arrange
        var positionId = Guid.NewGuid();
        var position = new PositionModel { Id = positionId, Name = positionName };

        this.db.BasicPositions.Add(position);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetPositionByIdQuery(positionId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(positionId, result.Id);
        Assert.Equal(positionName, result.Name);
    }

    [Fact]
    public async Task Handle_WhenPositionDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetPositionByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetPositionByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultiplePositions_ReturnsOnlyRequestedPosition()
    {
        // Arrange
        var position1Id = Guid.NewGuid();
        var position2Id = Guid.NewGuid();

        var position1 = new PositionModel { Id = position1Id, Name = "Developer" };
        var position2 = new PositionModel { Id = position2Id, Name = "Designer" };

        this.db.BasicPositions.AddRange(position1, position2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetPositionByIdQuery(position1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(position1Id, result.Id);
        Assert.Equal("Developer", result.Name);
        Assert.NotEqual("Designer", result.Name);
    }
}