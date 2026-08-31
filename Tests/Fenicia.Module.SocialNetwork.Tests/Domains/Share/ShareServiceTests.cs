using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Share;
using Fenicia.Module.SocialNetwork.Domains.Share.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Share;

public class ShareServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ShareService _service;
    private readonly Guid _companyId;

    public ShareServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ShareService(new ShareRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task ShareAsync_WhenCommandIsValid_CreatesShare()
    {
        // Arrange
        var command = new ShareCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence());

        // Act
        var result = await _service.ShareAsync(command, _companyId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.OriginalFeedId.Should().Be(command.OriginalFeedId);
        result.Text.Should().Be(command.Text);
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task GetSharesByFeedAsync_WhenSharesExist_ReturnsShares()
    {
        // Arrange
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
        var result = await _service.GetSharesByFeedAsync(new GetSharesByFeedQuery(1, 10), feedId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(share.Id);
        result.First().OriginalFeedId.Should().Be(feedId);
    }

    [Fact]
    public async Task GetSharesByFeedAsync_WhenNoSharesExist_ReturnsEmptyList()
    {
        // Arrange
        var feedId = Guid.NewGuid();

        // Act
        var result = await _service.GetSharesByFeedAsync(new GetSharesByFeedQuery(1, 10), feedId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
