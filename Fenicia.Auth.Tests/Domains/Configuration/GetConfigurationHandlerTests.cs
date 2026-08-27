using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.DTOs.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class GetConfigurationHandlerTests
{
    private readonly DefaultContext db;
    private readonly ConfigurationService service;
    private readonly Guid testUserId;

    public GetConfigurationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        service = new ConfigurationService(db);
        testUserId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var query = new GetConfigurationQuery(testUserId, companyId);

        var result = await service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WhenUserHasConfigurations_ReturnsAllConfigurations()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        db.AuthConfigurations.AddRange(config1, config2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(testUserId, companyId);

        var result = await service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[1].ConfigType);
    }

    [Fact]
    public async Task Handle_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var otherUserId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
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

        db.AuthConfigurations.AddRange(userConfig, otherUserConfig);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(testUserId, companyId);

        var result = await service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal("en", result[0].Value);
    }

    [Fact]
    public async Task Handle_WithNonExistentCompanyId_ReturnsEmptyList()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var config = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        db.AuthConfigurations.Add(config);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(Guid.NewGuid(), companyId);

        var result = await service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ConfigurationsAreOrderedByConfigType()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "GMT-3"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config3 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        db.AuthConfigurations.AddRange(config1, config2, config3);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(testUserId, companyId);

        var result = await service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Language, result[1].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[2].ConfigType);
    }

    [Fact]
    public async Task Handle_ResponseContainsCorrectData()
    {

        var configId = Guid.NewGuid();
        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var config = new ConfigurationModel
        {
            Id = configId,
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-bR"
        };

        db.AuthConfigurations.Add(config);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(testUserId, companyId);

        var result = await service.GetAllAsync(query.UserId, query.CompanyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(configId, result[0].Id);
        Assert.Equal(testUserId, result[0].UserId);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal("pt-bR", result[0].Value);
    }
}
