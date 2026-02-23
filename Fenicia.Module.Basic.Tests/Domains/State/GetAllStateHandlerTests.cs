using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.State.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.State;

[TestFixture]
public class GetAllStateHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetAllStateHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetAllStateHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllStateQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
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

        this.context.States.AddRange(state1, state2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllStateQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Any(s => s.Id == state1.Id));
            Assert.That(result.Any(s => s.Id == state2.Id));
        }
    }

    [Test]
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
            this.context.States.Add(state);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllStateQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(27));
    }

    [Test]
    public async Task Handle_VerifiesStateDataIsCorrect()
    {
        // Arrange
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "Minas Gerais",
            Uf = "MG"
        };

        this.context.States.Add(state);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllStateQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Name, Is.EqualTo("Minas Gerais"));
            Assert.That(result[0].Uf, Is.EqualTo("MG"));
        }
    }
}
