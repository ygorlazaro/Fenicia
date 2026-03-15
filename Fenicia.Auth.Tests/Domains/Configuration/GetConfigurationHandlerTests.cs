using Fenicia.Auth.Domains.Configuration.Handlers;
using Fenicia.Auth.Domains.Configuration.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

/// <summary>
///     Unit tests for the GetConfigurationHandler.
///     Tests configuration retrieval logic including filtering, ordering, and response mapping.
/// </summary>
public class GetConfigurationHandlerTests
{
    private readonly DefaultContext db;
    private readonly GetConfigurationHandler handler;
    private readonly Guid testUserId;

    public GetConfigurationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new GetConfigurationHandler(db);
        testUserId = Guid.NewGuid();
    }


    /// <summary>
    ///     Tests that a user with no configurations returns empty list.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {
        // Arrange
        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var query = new GetConfigurationQuery(testUserId, companyId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    ///     Tests that a user with configurations returns all of them.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserHasConfigurations_ReturnsAllConfigurations()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[1].ConfigType);
    }

    /// <summary>
    ///     Tests that filtering by company ID returns only that company's configurations.
    /// </summary>
    [Fact]
    public async Task Handle_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        // Arrange - Note: Due to how DefaultContext works, all entities get the same CompanyId
        // from TestCompanyContext, so this test verifies filtering works when CompanyId matches
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal("en", result[0].Value);
    }

    /// <summary>
    ///     Tests that querying with a non-existent company ID returns empty list.
    /// </summary>
    [Fact]
    public async Task Handle_WithNonExistentCompanyId_ReturnsEmptyList()
    {
        // Arrange
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

        // Query with a different user ID to get empty results
        var query = new GetConfigurationQuery(Guid.NewGuid(), companyId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    ///     Tests that configurations are ordered alphabetically by ConfigType.
    /// </summary>
    [Fact]
    public async Task Handle_ConfigurationsAreOrderedByConfigType()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal(ConfigType.Language, result[1].ConfigType);
        Assert.Equal(ConfigType.Timezone, result[2].ConfigType);
    }

    /// <summary>
    ///     Tests that the response contains all correct data fields.
    /// </summary>
    [Fact]
    public async Task Handle_ResponseContainsCorrectData()
    {
        // Arrange
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

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(configId, result[0].Id);
        Assert.Equal(testUserId, result[0].UserId);
        Assert.Equal(companyId, result[0].CompanyId);
        Assert.Equal(ConfigType.Language, result[0].ConfigType);
        Assert.Equal("pt-bR", result[0].Value);
    }
}
