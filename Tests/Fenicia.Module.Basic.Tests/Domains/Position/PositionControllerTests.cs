using System.Security.Claims;

using AwesomeAssertions;
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
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddPositionCommand(Guid.NewGuid(), _faker.Commerce.Department());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenPositionExists_ReturnsOk()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = "Test Position", CompanyId = _companyContext.CompanyId };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(position.Id, "Updated Name");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, position.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdatePositionCommand(Guid.NewGuid(), "Updated Name");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_ReturnsNoContent()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = "Test Position", CompanyId = _companyContext.CompanyId };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(position.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenPositionsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsOk()
    {
        // Arrange
        var position = new PositionModel { Id = Guid.NewGuid(), Name = "Test Position", CompanyId = _companyContext.CompanyId };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(position.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
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
