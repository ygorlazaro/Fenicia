using Fenicia.Auth.Domains.State;
using Fenicia.Common.Database.Models.Auth;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class StateServiceTests
{
    private Mock<IStateRepository> repoMock = null!;
    private StateService? sut;

    [SetUp]
    public void Setup()
    {
        repoMock = new Mock<IStateRepository>();
        sut = new StateService(repoMock.Object);
    }

    [Test]
    public async Task LoadStatesAtDatabaseAsync_MapsRepositoryModelsToResponses()
    {
        var states = new List<StateModel>
        {
            new() { Name = "Aland", Uf = "AL" },
            new() { Name = "Bord", Uf = "BO" }
        };

        repoMock.Setup(r => r.LoadStatesAtDatabaseAsync(It.IsAny<List<StateModel>>(), It.IsAny<CancellationToken>())).ReturnsAsync(states);

        var result = await sut!.LoadStatesAtDatabaseAsync(CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Any(r => r is { Name: "Aland", Uf: "AL" }));
    }
}
