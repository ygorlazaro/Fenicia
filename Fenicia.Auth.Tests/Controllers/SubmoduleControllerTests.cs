using Fenicia.Auth.Domains.Submodule;
using Fenicia.Common.API;
using Fenicia.Common.Data.Responses.Auth;

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
        this.submoduleServiceMock = new Mock<ISubmoduleService>();
        this.sut = new SubmoduleController(this.submoduleServiceMock.Object);
    }

    [Test]
    public async Task GetByModuleIdAsync_ReturnsOk()
    {
        var moduleId = Guid.NewGuid();
        var submodules = new List<SubmoduleResponse> { new() };

        this.submoduleServiceMock.Setup(x => x.GetByModuleIdAsync(moduleId, CancellationToken.None)).ReturnsAsync(submodules);

        var wide = new WideEventContext();

        var result = await this.sut.GetByModuleIdAsync(moduleId, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(submodules));
    }
}