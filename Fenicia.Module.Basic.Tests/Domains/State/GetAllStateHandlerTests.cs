using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class GetAllStateHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllStateHandler handler;

    public GetAllStateHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllStateHandler(db);
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
        // Act
        var result = await handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithStates_ReturnsAllStates()
    {
        // Arrange
        var state1 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };

        var state2 = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "Rio de Janeiro",
            Uf = "RJ"
        };

        db.AuthStates.AddRange(state1, state2);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Id == state1.Id);
        Assert.Contains(result, s => s.Id == state2.Id);
    }

    [Fact]
    public async Task Handle_WithMultipleStates_ReturnsAllWithoutPagination()
    {
        // Arrange
        for (var i = 0; i < 27; i++)
        {
            var state = new StateModel
            {
                Id = Guid.NewGuid(),
                Name = $"{faker.Address.State()} {i}",
                Uf = faker.Random.String2(2)
                    .ToUpper()
            };
            db.AuthStates.Add(state);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27, result.Count);
    }

    [Fact]
    public async Task Handle_VerifiesStateDataIsCorrect()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "Minas Gerais",
            Uf = "MG"
        };

        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Minas Gerais", result[0].Name);
        Assert.Equal("MG", result[0].Uf);
    }
}
