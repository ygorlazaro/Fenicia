using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Tests.Domains.Position;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionControllerTests : IDisposable
{
    private readonly PositionController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TestCompanyContext _companyContext;

    public PositionControllerTests()
    {
        _companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, _companyContext);
        var service = new PositionService(new PositionRepository(_db));
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new PositionController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenPositionsExist_ReturnsOk()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsOk()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = "Test Position", CompanyId = _companyContext.CompanyId };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var result = await _controller.GetByIdAsync(position.Id, wide, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);
        Assert.IsType<NotFoundResult>(result.Result);
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
