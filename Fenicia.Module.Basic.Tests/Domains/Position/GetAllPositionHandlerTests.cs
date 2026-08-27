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
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllPositionHandler handler;

    public GetAllPositionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllPositionHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {

        var query = new GetAllPositionQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithPositions_ReturnsAllPositions()
    {

        var position1 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Developer"
        };
        var position2 = new PositionModel
        {
            Id = Guid.NewGuid(),
            Name = "Designer"
        };

        db.BasicPositions.AddRange(position1, position2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Contains(result.Data, p => p.Id == position1.Id);
        Assert.Contains(result.Data, p => p.Id == position2.Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        for (var i = 0; i < 25; i++)
        {
            var position = new PositionModel
            {
                Id = Guid.NewGuid(),
                Name = $"{faker.Commerce.Department()} {i}"
            };
            db.BasicPositions.Add(position);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        for (var i = 0; i < 5; i++)
        {
            var position = new PositionModel
            {
                Id = Guid.NewGuid(),
                Name = $"{faker.Commerce.Department()} {i}"
            };
            db.BasicPositions.Add(position);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        for (var i = 0; i < 25; i++)
        {
            var position = new PositionModel
            {
                Id = Guid.NewGuid(),
                Name = $"{faker.Commerce.Department()} {i}"
            };
            db.BasicPositions.Add(position);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllPositionQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
    }
}
