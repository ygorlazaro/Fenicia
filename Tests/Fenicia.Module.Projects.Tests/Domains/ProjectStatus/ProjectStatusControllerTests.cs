using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class ProjectStatusControllerTests : IDisposable
{
    private readonly ProjectStatusController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public ProjectStatusControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectStatusRepository(_db);
        var service = new ProjectStatusService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectStatusController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _testUserId = Guid.NewGuid();
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenStatusesExist_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
