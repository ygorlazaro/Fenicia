using Fenicia.Auth.Domains.Module;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class ModuleControllerTests
{
    private Mock<IModuleService> moduleServiceMock;
    private ModuleController sut;

    [SetUp]
    public void Setup()
    {
        this.moduleServiceMock = new Mock<IModuleService>();
        this.sut = new ModuleController();
    }

    [Test]
    public async Task GetAllModulesAsync_ReturnsPagination()
    {
        var modules = new List<ModuleResponse> { new() { Id = Guid.NewGuid(), Name = "M" } };
        this.moduleServiceMock.Setup(x => x.GetAllOrderedAsync(CancellationToken.None, 1, 10)).ReturnsAsync(modules);
        this.moduleServiceMock.Setup(x => x.CountAsync(CancellationToken.None)).ReturnsAsync(1);

        var query = new PaginationQuery { Page = 1, PerPage = 10 };
        var wide = new WideEventContext();

        var result = await this.sut.GetAllModulesAsync(query, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok.Value, Is.TypeOf<Pagination<List<ModuleResponse>>>());
    }
}
