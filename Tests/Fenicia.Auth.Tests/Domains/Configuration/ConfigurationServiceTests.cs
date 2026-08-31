using Bogus;

using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ConfigurationService _service;
    private readonly Guid _testUserId;

    public ConfigurationServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        var repository = new ConfigurationRepository(_db);
        _service = new ConfigurationService(repository);
        _faker = new Faker();
        _testUserId = Guid.NewGuid();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var query = new GetConfigurationQuery(_testUserId, companyId);

        var result = await _service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasConfigurations_ReturnsAllConfigurations()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        _db.AuthConfigurations.AddRange(config1, config2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(_testUserId, companyId);

        var result = await _service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[1].ConfigType);
    }

    [Fact]
    public async Task GetAllAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var otherUserId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en"
        };

        var otherUserConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _db.AuthConfigurations.AddRange(userConfig, otherUserConfig);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(_testUserId, companyId);

        var result = await _service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal("en", result[0].Value);
    }

    [Fact]
    public async Task GetAllAsync_WithNonExistentCompanyId_ReturnsEmptyList()
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

        var query = new GetConfigurationQuery(Guid.NewGuid(), companyId);

        var result = await _service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ConfigurationsAreOrderedByConfigType()
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
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        _db.AuthConfigurations.AddRange(config1, config2, config3);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(_testUserId, companyId);

        var result = await _service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Language, result[1].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[2].ConfigType);
    }

    [Fact]
    public async Task GetAllAsync_ResponseContainsCorrectData()
    {
        var configId = Guid.NewGuid();
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var config = new ConfigurationModel
        {
            Id = configId,
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-bR"
        };

        _db.AuthConfigurations.Add(config);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(_testUserId, companyId);

        var result = await _service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(configId, result[0].Id);
        Assert.Equal(_testUserId, result[0].UserId);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal("pt-bR", result[0].Value);
    }

    [Fact]
    public async Task UpsertAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        await _service.UpsertAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);

        Assert.Equal(_testUserId, configuration.UserId);
        Assert.Equal(ConfigType.Language, configuration.ConfigType);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(_db.CurrentCompanyId, configuration.CompanyId);
    }

    [Fact]
    public async Task UpsertAsync_WithSameUserAndTypeButDifferentCompany_CreatesSeparateConfigurations()
    {
        var config1 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var config2 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Timezone, "dark");

        await _service.UpsertAsync(config1, _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);
        await _service.UpsertAsync(config2, _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);

        var configurations = await _db.AuthConfigurations.Where(c => c.UserId == _testUserId).ToListAsync(CancellationToken.None);

        Assert.Equal(2, configurations.Count);
    }

    [Fact]
    public async Task UpsertAsync_UpdatesConfigurationDoesNotChangeId()
    {
        var originalId = Guid.NewGuid();
        var companyId = _db.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = originalId,
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _db.AuthConfigurations.Add(existingConfig);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "en");

        await _service.UpsertAsync(command, companyId, CancellationToken.None);

        var updatedConfig = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.Id == originalId);

        Assert.NotNull(updatedConfig);

        Assert.Equal(originalId, updatedConfig.Id);
        Assert.Equal("en", updatedConfig.Value);
    }

    [Fact]
    public async Task UpsertAsync_MultipleUpdates_OnlyLastValuePersists()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;

        var command1 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var command2 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "en");

        var command3 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "es");

        await _service.UpsertAsync(command1, companyId, CancellationToken.None);
        await _service.UpsertAsync(command2, companyId, CancellationToken.None);
        await _service.UpsertAsync(command3, companyId, CancellationToken.None);

        var configurations = await _db.AuthConfigurations.Where(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId).ToListAsync(CancellationToken.None);

        Assert.Single(configurations);
        Assert.Equal("es", configurations[0].Value);
    }

    [Fact]
    public async Task UpsertAsync_WithEmptyValue_SavesEmptyString()
    {
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, string.Empty);

        await _service.UpsertAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(string.Empty, configuration.Value);
    }

    [Fact]
    public async Task UpsertAsync_WithLongValue_SavesSuccessfully()
    {
        var longValue = _faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, longValue);

        await _service.UpsertAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(longValue, configuration.Value);
    }
}
