using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class UpdatePositionServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly PositionService service;

    public UpdatePositionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new PositionService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_ReturnsUpdateResponse()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var newName = faker.Commerce.Categories(1).First();
        var command = new UpdatePositionCommand(position.Id, newName);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(position.Id, result.Id);
        Assert.Equal(newName, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        var command = new UpdatePositionCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.Null(result);
    }
}
