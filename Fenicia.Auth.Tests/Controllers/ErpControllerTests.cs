using Fenicia.Auth.Domains.ERP;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.State;
using Fenicia.Common.Database.Responses.Auth;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class ErpControllerTests
{
    private Mock<IModuleService> moduleServiceMock;
    private Mock<IStateService> stateServiceMock;
    private ErpController sut;

    [SetUp]
    public void Setup()
    {
        moduleServiceMock = new Mock<IModuleService>();
        stateServiceMock = new Mock<IStateService>();

        sut = new ErpController(moduleServiceMock.Object, stateServiceMock.Object);
    }

    [Test]
    public async Task LoadInfoAsyncReturnsModulesAndStates()
    {
        var modules = new List<ModuleResponse> { new() { Id = Guid.NewGuid(), Name = "M", Price = 0, Type = default } };
        var states = new List<StateResponse> { new() { Id = Guid.NewGuid(), Name = "S", Uf = "UF" } };

        moduleServiceMock.Setup(x => x.LoadModulesAtDatabaseAsync(CancellationToken.None)).ReturnsAsync(modules);
        stateServiceMock.Setup(x => x.LoadStatesAtDatabaseAsync(CancellationToken.None)).ReturnsAsync(states);

        var result = await sut.LoadInfoAsync(CancellationToken.None);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)result;
        Assert.That(ok.Value, Is.Not.Null);
        var valueType = ok.Value.GetType();
        var modulesProp = valueType.GetProperty("Modules");
        var statesProp = valueType.GetProperty("States");
        var gotModules = (IEnumerable<ModuleResponse>)modulesProp?.GetValue(ok.Value)!;
        var gotStates = (IEnumerable<StateResponse>)statesProp?.GetValue(ok.Value)!;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(gotModules.First().Id, Is.EqualTo(modules[0].Id));
            Assert.That(gotStates.First().Id, Is.EqualTo(states[0].Id));
        }
    }
}
