using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Block;
using Fenicia.Module.SocialNetwork.Domains.Block.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Block;

public class BlockServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly BlockService _service;

    public BlockServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new BlockRepository(_db);
        _service = new BlockService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task BlockAsync_WhenNotBlocked_CreatesBlock()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();

        var result = await _service.BlockAsync(new BlockCommand(blockedUserId), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.BlockedUserId.Should().Be(blockedUserId);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task BlockAsync_WhenAlreadyBlocked_ReturnsExistingBlock()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();
        var block = new BlockModel
        {
            UserId = userId,
            BlockedUserId = blockedUserId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkBlocks.Add(block);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.BlockAsync(new BlockCommand(blockedUserId), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(block.Id);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task BlockAsync_WhenPreviouslyUnblocked_ReactivatesBlock()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();
        var block = new BlockModel
        {
            UserId = userId,
            BlockedUserId = blockedUserId,
            BlockDate = DateTime.UtcNow,
            IsActive = false
        };
        _db.SocialNetworkBlocks.Add(block);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.BlockAsync(new BlockCommand(blockedUserId), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(block.Id);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UnblockAsync_WhenBlocked_SetsInactive()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();
        var block = new BlockModel
        {
            UserId = userId,
            BlockedUserId = blockedUserId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkBlocks.Add(block);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.UnblockAsync(new UnblockCommand(blockedUserId), userId, CancellationToken.None);

        var updated = await _db.SocialNetworkBlocks.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == block.Id);
        updated.Should().NotBeNull();
        updated!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UnblockAsync_WhenNotBlocked_DoesNothing()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();

        await _service.UnblockAsync(new UnblockCommand(blockedUserId), userId, CancellationToken.None);

        var count = await _db.SocialNetworkBlocks.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetBlockedAsync_WhenBlockedExist_ReturnsPagination()
    {
        var userId = Guid.NewGuid();
        var blocked = new BlockModel
        {
            UserId = userId,
            BlockedUserId = Guid.NewGuid(),
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkBlocks.Add(blocked);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetBlockedAsync(new GetBlockedQuery(1, 10, null, null), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().BlockedUserId.Should().Be(blocked.BlockedUserId);
    }

    [Fact]
    public async Task IsBlockedAsync_WhenBlocked_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();
        var block = new BlockModel
        {
            UserId = userId,
            BlockedUserId = blockedUserId,
            BlockDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkBlocks.Add(block);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.IsBlockedAsync(new IsBlockedQuery(blockedUserId), userId, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsBlockedAsync_WhenNotBlocked_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();

        var result = await _service.IsBlockedAsync(new IsBlockedQuery(blockedUserId), userId, CancellationToken.None);

        result.Should().BeFalse();
    }
}
