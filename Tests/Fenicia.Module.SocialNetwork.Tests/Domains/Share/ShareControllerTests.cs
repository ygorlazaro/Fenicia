using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Share;
using Fenicia.Module.SocialNetwork.Domains.Share.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Share;

public class ShareControllerTests : IDisposable
{
    private readonly ShareController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public ShareControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ShareRepository(_db);
        var service = new ShareService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ShareController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _testUserId = Guid.NewGuid();
        _companyId = companyContext.CompanyId;
        SetupUserClaims(_testUserId);
        _faker = new Faker();
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
        var command = new ShareCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedShare = (AddShareResponse)createdResult.Value!;
        returnedShare.Id.Should().Be(command.Id);
        returnedShare.OriginalFeedId.Should().Be(command.OriginalFeedId);
    }

    [Fact]
    public async Task GetSharesByFeedAsync_WhenSharesExist_ReturnsOkWithShares()
    {
        // Arrange
        var wide = new WideEventContext();
        var feedId = Guid.NewGuid();
        var share = new ShareModel
        {
            Id = Guid.NewGuid(),
            OriginalFeedId = feedId,
            Text = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.SocialNetworkShares.Add(share);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetSharesByFeedAsync(feedId, wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var shares = (List<GetSharesResponse>)okResult.Value!;
        shares.Should().HaveCount(1);
        shares.First().Id.Should().Be(share.Id);
    }

    [Fact]
    public async Task GetSharesByFeedAsync_WhenNoSharesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var feedId = Guid.NewGuid();

        // Act
        var result = await _controller.GetSharesByFeedAsync(feedId, wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var shares = (List<GetSharesResponse>)okResult.Value!;
        shares.Should().BeEmpty();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
