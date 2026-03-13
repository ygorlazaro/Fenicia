using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position.Handlers;
using Fenicia.Module.Basic.Domains.Position.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class GetAllPositionHandlerTests : IDisposable
{
    public GetAllPositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new GetAllPositionHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly GetAllPositionHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllPositionQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithPositions_ReturnsAllPositions()
    {
        // Arrange
        var position1 = new PositionModel { Id = Guid.NewGuid(), Name = "Developer" };
        var position2 = new PositionModel { Id = Guid.NewGuid(), Name = "Designer" };

        this.db.BasicPositions.AddRange(position1,
            position2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Data.Count);
        Assert.Contains(result.Data,
            p => p.Id == position1.Id);
        Assert.Contains(result.Data,
            p => p.Id == position2.Id);
    }

    [Fact]
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
            this.db.BasicPositions.Add(position);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery(2);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10,
            result.Data.Count);
    }

    [Fact]
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
            this.db.BasicPositions.Add(position);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery(10);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
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
            this.db.BasicPositions.Add(position);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10,
            result.Data.Count);
    }
}
