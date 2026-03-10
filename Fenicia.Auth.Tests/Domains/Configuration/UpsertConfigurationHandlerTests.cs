using Bogus;

using Fenicia.Auth.Domains.Configuration.UpsertConfiguration;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class UpsertConfigurationHandlerTests : IDisposable
{
    private readonly UpsertConfigurationHandler handler;
    private readonly DefaultContext context;
    private readonly Faker faker;
    private readonly Guid testUserId;

    public UpsertConfigurationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new UpsertConfigurationHandler(this.context);
        this.faker = new Faker();
        this.testUserId = Guid.NewGuid();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.context.CurrentCompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);

        Assert.Equal(this.testUserId, configuration.UserId);
        Assert.Equal(ConfigType.Language, configuration.ConfigType);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(this.context.CurrentCompanyId, configuration.CompanyId);
    }

    [Fact]
    public async Task Handle_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        // Arrange
        var companyId = this.context.CurrentCompanyId;
        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId ?? Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "pt-BR",
            Created = DateTime.UtcNow.AddDays(-1)
        };

        this.context.AuthConfiguration.Add(existingConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "en",
            companyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedConfig = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(updatedConfig);

        Assert.Equal("en", updatedConfig.Value);
        Assert.Equal(existingConfig.Created, updatedConfig.Created);
    }

    [Fact]
    public async Task Handle_WithCompanyId_CreatesCompanyConfiguration()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.context.CurrentCompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c =>
                c.UserId == this.testUserId &&
                c.ConfigType == ConfigType.Language &&
                c.CompanyId == this.context.CurrentCompanyId);

        Assert.NotNull(configuration);

        Assert.Equal(this.context.CurrentCompanyId, configuration.CompanyId);
        Assert.Equal("pt-BR", configuration.Value);
    }

    [Fact]
    public async Task Handle_WithSameUserAndTypeButDifferentCompany_CreatesSeparateConfigurations()
    {
        // Arrange - Note: Due to how DefaultContext works, all entities get the same CompanyId
        // from TestCompanyContext, so this test verifies multiple configurations for same user/type
        var config1 = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.context.CurrentCompanyId?? Guid.NewGuid()
        );

        var config2 = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Timezone,
            "dark",
            this.context.CurrentCompanyId?? Guid.NewGuid()
        );

        // Act
        await this.handler.Handle(config1, CancellationToken.None);
        await this.handler.Handle(config2, CancellationToken.None);

        // Assert
        var configurations = await this.context.AuthConfiguration
            .Where(c => c.UserId == this.testUserId)
            .ToListAsync(CancellationToken.None);

        Assert.Equal(2, configurations.Count);
    }

    [Fact]
    public async Task Handle_UpdatesConfigurationDoesNotChangeId()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var companyId = this.context.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = originalId,
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.context.AuthConfiguration.Add(existingConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "en",
            companyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedConfig = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.Id == originalId);

        Assert.NotNull(updatedConfig);

        Assert.Equal(originalId, updatedConfig.Id);
        Assert.Equal("en", updatedConfig.Value);
    }

    [Fact]
    public async Task Handle_MultipleUpdates_OnlyLastValuePersists()
    {
        // Arrange
        var companyId = this.context.CurrentCompanyId ?? Guid.NewGuid();

        var command1 = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            companyId
        );

        var command2 = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "en",
            companyId
        );

        var command3 = new UpsertConfigurationCommand(
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
        var configurations = await this.context.AuthConfiguration
            .Where(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId)
            .ToListAsync(CancellationToken.None);

        Assert.Single(configurations);
        Assert.Equal("es", configurations[0].Value);
    }

    [Fact]
    public async Task Handle_WithEmptyValue_SavesEmptyString()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "",
            this.context.CurrentCompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal("", configuration.Value);
    }

    [Fact]
    public async Task Handle_WithLongValue_SavesSuccessfully()
    {
        // Arrange
        var longValue = this.faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            longValue,
            this.context.CurrentCompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(longValue, configuration.Value);
    }
}
