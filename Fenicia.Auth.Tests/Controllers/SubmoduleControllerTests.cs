using Fenicia.Auth.Domains.Submodule;
using Fenicia.Common;
using Fenicia.Common.Api;
using Fenicia.Common.Database.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class SubmoduleControllerTests
{
    private Mock<ISubmoduleService> submoduleServiceMock;
    private SubmoduleController sut;

    [SetUp]
    public void Setup()
    {
        submoduleServiceMock = new Mock<ISubmoduleService>();
        sut = new SubmoduleController(submoduleServiceMock.Object);
    }

    [Test]
    public async Task GetByModuleIdAsync_ReturnsOk()
    {
        var moduleId = Guid.NewGuid();
        var submodules = new List<SubmoduleResponse> { new() };

        submoduleServiceMock.Setup(x => x.GetByModuleIdAsync(moduleId, CancellationToken.None)).ReturnsAsync(submodules);

        var wide = new WideEventContext();

        var result = await sut.GetByModuleIdAsync(moduleId, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(submodules));
    }
}
