using Fenicia.Auth.Domains.Configuration;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly ConfigurationRepository _repository;
    private readonly Guid _testUserId;

    public ConfigurationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new ConfigurationRepository(_db);
        _testUserId = Guid.NewGuid();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByUserCompanyAndTypeAsync_WhenConfigurationExists_ReturnsConfiguration()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var config = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _db.AuthConfigurations.Add(config);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByUserCompanyAndTypeAsync(
            _testUserId,
            companyId,
            ConfigType.Language,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(config.Id, result.Id);
        Assert.Equal("pt-BR", result.Value);
    }

    [Fact]
    public async Task GetByUserCompanyAndTypeAsync_WhenConfigurationDoesNotExist_ReturnsNull()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;

        var result = await _repository.GetByUserCompanyAndTypeAsync(
            _testUserId,
            companyId,
            ConfigType.Language,
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserCompanyAndTypeAsync_WithDifferentUser_ReturnsNull()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var otherUserId = Guid.NewGuid();
        var config = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _db.AuthConfigurations.Add(config);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByUserCompanyAndTypeAsync(
            _testUserId,
            companyId,
            ConfigType.Language,
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserAndCompanyAsync_ReturnsOrderedConfigurations()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "GMT-3"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config3 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "UTC"
        };

        _db.AuthConfigurations.AddRange(config1, config2, config3);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByUserAndCompanyAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[1].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[2].ConfigType);
    }

    [Fact]
    public async Task GetByUserAndCompanyAsync_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;

        var result = await _repository.GetByUserAndCompanyAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserAndCompanyAsync_WithDifferentCompany_ReturnsEmptyList()
    {
        var companyId1 = _db.CurrentCompanyId ?? Guid.Empty;
        var companyId2 = Guid.NewGuid();

        var config = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId1,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _db.AuthConfigurations.Add(config);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByUserAndCompanyAsync(_testUserId, companyId2, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserAndCompanyAsync_WithDifferentUser_ReturnsOnlyRequestedUserConfigurations()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var otherUserId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var otherUserConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        _db.AuthConfigurations.AddRange(userConfig, otherUserConfig);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByUserAndCompanyAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(_testUserId, result[0].UserId);
    }
}