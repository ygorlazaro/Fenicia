using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Share;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Share;

public class ShareRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ShareRepository _repository;
    private readonly Guid _companyId;

    public ShareRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new ShareRepository(_db);
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllShares()
    {
        // Arrange
        var share = new ShareModel
        {
            Id = Guid.NewGuid(),
            OriginalFeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.SocialNetworkShares.Add(share);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenShareExists_ReturnsShare()
    {
        // Arrange
        var share = new ShareModel
        {
            Id = Guid.NewGuid(),
            OriginalFeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.SocialNetworkShares.Add(share);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(share.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(share.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenShareDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenShareIsValid_InsertsShare()
    {
        // Arrange
        var share = new ShareModel
        {
            Id = Guid.NewGuid(),
            OriginalFeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };

        // Act
        var result = await _repository.InsertAsync(share, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task DeleteAsync_WhenShareExists_SoftDeletesShare()
    {
        // Arrange
        var share = new ShareModel
        {
            Id = Guid.NewGuid(),
            OriginalFeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.SocialNetworkShares.Add(share);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(share.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedShare = await _db.SocialNetworkShares.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == share.Id);
        deletedShare.Should().NotBeNull();
        deletedShare!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSharesByFeedAsync_ReturnsSharesByFeed()
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
        var result = await _repository.GetSharesByFeedAsync(1, 10, feedId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
