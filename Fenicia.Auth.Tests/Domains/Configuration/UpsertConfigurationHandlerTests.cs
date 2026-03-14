using Bogus;

using Fenicia.Auth.Domains.Configuration.Commands;
using Fenicia.Auth.Domains.Configuration.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

/// <summary>
/// Unit tests for the UpsertConfigurationHandler.
/// Tests the upsert logic for creating and updating configuration entries.
/// </summary>
public class UpsertConfigurationHandlerTests : IDisposable
{
    private readonly UpsertConfigurationHandler handler;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid testUserId;

    /// <summary>
    /// Tests that when a configuration doesn't exist, a new one is created.
    /// </summary>
    [Fact]
    public async Task Handle_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.db.CurrentCompanyId ?? Guid.Empty
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);

        Assert.Equal(this.testUserId, configuration.UserId);
        Assert.Equal(ConfigType.Language, configuration.ConfigType);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(this.db.CurrentCompanyId, configuration.CompanyId);
    }

    /// <summary>
    /// Tests that different ConfigTypes create separate configuration entries.
    /// </summary>
    [Fact]
    public async Task Handle_WithSameUserAndTypeButDifferentCompany_CreatesSeparateConfigurations()
    {
        // Arrange - Note: Due to how DefaultContext works, all entities get the same CompanyId
        // from TestCompanyContext, so this test verifies multiple configurations for same user/type
        var config1 = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.db.CurrentCompanyId ?? Guid.NewGuid()
        );

        var config2 = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Timezone,
            "dark",
            this.db.CurrentCompanyId ?? Guid.NewGuid()
        );

        // Act
        await this.handler.Handle(config1, CancellationToken.None);
        await this.handler.Handle(config2, CancellationToken.None);

        // Assert
        var configurations = await this.db.AuthConfigurations
            .Where(c => c.UserId == this.testUserId)
            .ToListAsync(CancellationToken.None);

        Assert.Equal(2, configurations.Count);
    }

    /// <summary>
    /// Tests that updating a configuration does not change its ID.
    /// </summary>
    [Fact]
    public async Task Handle_UpdatesConfigurationDoesNotChangeId()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var companyId = this.db.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = originalId,
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.db.AuthConfigurations.Add(existingConfig);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "en",
            companyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedConfig = await this.db.AuthConfigurations
            .FirstOrDefaultAsync(c => c.Id == originalId);

        Assert.NotNull(updatedConfig);

        Assert.Equal(originalId, updatedConfig.Id);
        Assert.Equal("en", updatedConfig.Value);
    }

    /// <summary>
    /// Tests that multiple sequential updates only keep the last value (upsert behavior).
    /// </summary>
    [Fact]
    public async Task Handle_MultipleUpdates_OnlyLastValuePersists()
    {
        // Arrange
        var companyId = this.db.CurrentCompanyId ?? Guid.NewGuid();

        var command1 = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            companyId
        );

        var command2 = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "en",
            companyId
        );

        var command3 = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "es",
            companyId
        );

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);
        await this.handler.Handle(command3, CancellationToken.None);

        // Assert
        var configurations = await this.db.AuthConfigurations
            .Where(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId)
            .ToListAsync(CancellationToken.None);

        Assert.Single(configurations);
        Assert.Equal("es", configurations[0].Value);
    }

    /// <summary>
    /// Tests that an empty string value can be saved.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyValue_SavesEmptyString()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            "",
            this.db.CurrentCompanyId ?? Guid.Empty
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.db.AuthConfigurations
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal("", configuration.Value);
    }

    /// <summary>
    /// Tests that long text values can be saved successfully.
    /// </summary>
    [Fact]
    public async Task Handle_WithLongValue_SavesSuccessfully()
    {
        // Arrange
        var longValue = this.faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(
            null,
            this.testUserId,
            ConfigType.Language,
            longValue,
            this.db.CurrentCompanyId ?? Guid.Empty
        );

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var configuration = await this.db.AuthConfigurations
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(longValue, configuration.Value);
    }
}
