using System.Security.Claims;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.API;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class UserControllerTests
{
    private Mock<IModuleService> moduleServiceMock;
    private UserController sut;
    private Mock<IUserRoleService> userRoleServiceMock;

    [SetUp]
    public void Setup()
    {
        this.moduleServiceMock = new Mock<IModuleService>();
        this.userRoleServiceMock = new Mock<IUserRoleService>();
        this.sut = new UserController(this.moduleServiceMock.Object, this.userRoleServiceMock.Object);
    }

    [Test]
    public async Task GetUserModulesAsync_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var headers = new Headers { CompanyId = Guid.NewGuid() };
        var modules = new List<ModuleResponse> { new() { Id = Guid.NewGuid(), Name = "M" } };

        this.moduleServiceMock
            .Setup(x => x.GetModuleAndSubmoduleAsync(userId, headers.CompanyId, CancellationToken.None))
            .ReturnsAsync(modules);

        this.sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        this.sut.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var wide = new WideEventContext();

        var result = await this.sut.GetUserModulesAsync(headers, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(modules));
    }

    [Test]
    public async Task GetUserCompanyAsync_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var companies = new List<CompanyResponse> { new() { Id = Guid.NewGuid(), Name = "C" } };

        this.userRoleServiceMock.Setup(x => x.GetUserCompaniesAsync(userId, CancellationToken.None))
            .ReturnsAsync(companies);

        this.sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        this.sut.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var wide = new WideEventContext();

        var result = await this.sut.GetUserCompanyAsync(wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(companies));
    }
}