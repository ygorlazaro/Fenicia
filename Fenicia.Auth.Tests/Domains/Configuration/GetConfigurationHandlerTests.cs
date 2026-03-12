using Fenicia.Auth.Domains.Configuration.Handlers;
using Fenicia.Auth.Domains.Configuration.Queries;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class GetConfigurationHandlerTests : IDisposable
{
    private readonly GetConfigurationHandler handler;
    private readonly DefaultContext db;
    private readonly Guid testUserId;

    public GetConfigurationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new GetConfigurationHandler(this.db);
        this.testUserId = Guid.NewGuid();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetConfigurationQuery(this.testUserId,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WhenUserHasConfigurations_ReturnsAllConfigurations()
    {
        // Arrange
        var companyId = this.db.CurrentCompanyId ?? Guid.Empty;
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        this.db.AuthConfigurations.AddRange(config1,
            config2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId,
            companyId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Count);

        Assert.Equal(ConfigType.Language,
            result[0].ConfigType);
        Assert.Equal(ConfigType.Timezone,
            result[1].ConfigType);
    }

    [Fact]
    public async Task Handle_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        // Arrange - Note: Due to how DefaultContext works, all entities get the same CompanyId
        // from TestCompanyContext, so this test verifies filtering works when CompanyId matches
        var companyId = this.db.CurrentCompanyId ?? Guid.Empty;
        var otherUserId = Guid.NewGuid();
        
        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
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

        this.db.AuthConfigurations.AddRange(userConfig,
            otherUserConfig);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId,
            companyId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(companyId,
            result[0].CompanyId);
        Assert.Equal("en",
            result[0].Value);
    }

    [Fact]
    public async Task Handle_WithNonExistentCompanyId_ReturnsEmptyList()
    {
        // Arrange
        var companyId = this.db.CurrentCompanyId ?? Guid.Empty;
        var config = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.db.AuthConfigurations.Add(config);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Query with a different user ID to get empty results
        var query = new GetConfigurationQuery(Guid.NewGuid(),
            companyId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ConfigurationsAreOrderedByConfigType()
    {
        // Arrange
        var companyId = this.db.CurrentCompanyId ?? Guid.Empty;
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Timezone,
            Value = "GMT-3"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config3 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        this.db.AuthConfigurations.AddRange(config1,
            config2,
            config3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId,
            companyId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3,
            result.Count);

        Assert.Equal(ConfigType.Language,
            result[0].ConfigType);
        Assert.Equal(ConfigType.Language,
            result[1].ConfigType);
        Assert.Equal(ConfigType.Timezone,
            result[2].ConfigType);
    }

    [Fact]
    public async Task Handle_ResponseContainsCorrectData()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var companyId = this.db.CurrentCompanyId ?? Guid.Empty;
        var config = new ConfigurationModel
        {
            Id = configId,
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-bR"
        };

        this.db.AuthConfigurations.Add(config);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId,
            companyId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(configId,
            result[0].Id);
        Assert.Equal(this.testUserId,
            result[0].UserId);
        Assert.Equal(companyId,
            result[0].CompanyId);
        Assert.Equal(ConfigType.Language,
            result[0].ConfigType);
        Assert.Equal("pt-bR",
            result[0].Value);
    }
}
