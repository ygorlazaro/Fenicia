using Bogus;
using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IConfigurationRepository> _mockRepository;
    private readonly ConfigurationService _service;
    private readonly Guid _testUserId;

    public ConfigurationServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IConfigurationRepository>();
        _service = new ConfigurationService(_mockRepository.Object);
        _testUserId = Guid.NewGuid();
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByUserAndCompanyAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetAllAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasConfigurations_ReturnsAllConfigurations()
    {
        var companyId = Guid.NewGuid();
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

        _mockRepository.Setup(r => r.GetByUserAndCompanyAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([config1, config2]);

        var result = await _service.GetAllAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[1].ConfigType);
    }

    [Fact]
    public async Task GetAllAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        var companyId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en"
        };

        _mockRepository.Setup(r => r.GetByUserAndCompanyAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userConfig]);

        var result = await _service.GetAllAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal("en", result[0].Value);
    }

    [Fact]
    public async Task GetAllAsync_WithNonExistentCompanyId_ReturnsEmptyList()
    {
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByUserAndCompanyAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetAllAsync(_testUserId, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ConfigurationsAreOrderedByConfigType()
    {
        var companyId = Guid.NewGuid();
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

        _mockRepository.Setup(r => r.GetByUserAndCompanyAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([config1, config2, config3]);

        var result = await _service.GetAllAsync(_testUserId, companyId, CancellationToken.None);

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
        var companyId = Guid.NewGuid();
        var config = new ConfigurationModel
        {
            Id = configId,
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-bR"
        };

        _mockRepository.Setup(r => r.GetByUserAndCompanyAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([config]);

        var result = await _service.GetAllAsync(_testUserId, companyId, CancellationToken.None);

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
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByUserCompanyAndTypeAsync(
                _testUserId,
                companyId,
                ConfigType.Language,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfigurationModel?)null);

        await _service.UpsertAsync(command, companyId, CancellationToken.None);

        _mockRepository.Verify(
            r => r.InsertAsync(It.IsAny<ConfigurationModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpsertAsync_UpdatesConfigurationDoesNotChangeId()
    {
        var originalId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = originalId,
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _mockRepository.Setup(r => r.GetByUserCompanyAndTypeAsync(
                _testUserId,
                companyId,
                ConfigType.Language,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);
        _mockRepository.Setup(r => r.UpdateAsync(
                originalId,
                It.IsAny<ConfigurationModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);

        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "en");

        await _service.UpsertAsync(command, companyId, CancellationToken.None);

        _mockRepository.Verify(
            r => r.UpdateAsync(originalId, It.IsAny<ConfigurationModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpsertAsync_MultipleUpdates_OnlyLastValuePersists()
    {
        var companyId = Guid.NewGuid();

        var command1 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");
        var command2 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "en");
        var command3 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "es");

        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _mockRepository.Setup(r => r.GetByUserCompanyAndTypeAsync(
                _testUserId,
                companyId,
                ConfigType.Language,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);
        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Guid>(),
                It.IsAny<ConfigurationModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);

        await _service.UpsertAsync(command1, companyId, CancellationToken.None);
        await _service.UpsertAsync(command2, companyId, CancellationToken.None);
        await _service.UpsertAsync(command3, companyId, CancellationToken.None);

        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ConfigurationModel>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task UpsertAsync_WithEmptyValue_SavesEmptyString()
    {
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, string.Empty);
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByUserCompanyAndTypeAsync(
                _testUserId,
                companyId,
                ConfigType.Language,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfigurationModel?)null);

        await _service.UpsertAsync(command, companyId, CancellationToken.None);

        _mockRepository.Verify(
            r => r.InsertAsync(It.Is<ConfigurationModel>(c => c.Value == string.Empty), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpsertAsync_WithLongValue_SavesSuccessfully()
    {
        var longValue = _faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, longValue);
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByUserCompanyAndTypeAsync(
                _testUserId,
                companyId,
                ConfigType.Language,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfigurationModel?)null);

        await _service.UpsertAsync(command, companyId, CancellationToken.None);

        _mockRepository.Verify(
            r => r.InsertAsync(It.Is<ConfigurationModel>(c => c.Value == longValue), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}