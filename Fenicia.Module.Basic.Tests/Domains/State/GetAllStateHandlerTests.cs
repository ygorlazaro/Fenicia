using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.State.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class GetAllStateHandlerTests : IDisposable
{
    public GetAllStateHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new GetAllStateHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly GetAllStateHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await this.handler.Handle(CancellationToken.None);

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

        this.db.AuthStates.AddRange(state1,
            state2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Count);
        Assert.Contains(result,
            s => s.Id == state1.Id);
        Assert.Contains(result,
            s => s.Id == state2.Id);
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
                Name = $"{this.faker.Address.State()} {i}",
                Uf = this.faker.Random.String2(2).ToUpper()
            };
            this.db.AuthStates.Add(state);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(27,
            result.Count);
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

        this.db.AuthStates.Add(state);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Minas Gerais",
            result[0].Name);
        Assert.Equal("MG",
            result[0].Uf);
    }
}
