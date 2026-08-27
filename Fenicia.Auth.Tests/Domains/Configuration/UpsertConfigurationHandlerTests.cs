using Bogus;

using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class UpsertConfigurationServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ConfigurationService service;
    private readonly Guid testUserId;

    public UpsertConfigurationServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        service = new ConfigurationService(db);
        faker = new Faker();
        testUserId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {

        var command = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", db.CurrentCompanyId ?? Guid.Empty);

        await service.UpsertAsync(command, CancellationToken.None);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);

        Assert.Equal(testUserId, configuration.UserId);
        Assert.Equal(ConfigType.Language, configuration.ConfigType);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(db.CurrentCompanyId, configuration.CompanyId);
    }

    [Fact]
    public async Task Handle_WithSameUserAndTypeButDifferentCompany_CreatesSeparateConfigurations()
    {

        var config1 = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", db.CurrentCompanyId ?? Guid.NewGuid());

        var config2 = new UpsertConfigurationCommand(null, testUserId, ConfigType.Timezone, "dark", db.CurrentCompanyId ?? Guid.NewGuid());

        await service.UpsertAsync(config1, CancellationToken.None);
        await service.UpsertAsync(config2, CancellationToken.None);

        var configurations = await db.AuthConfigurations.Where(c => c.UserId == testUserId).ToListAsync(CancellationToken.None);

        Assert.Equal(2, configurations.Count);
    }

    [Fact]
    public async Task Handle_UpdatesConfigurationDoesNotChangeId()
    {

        var originalId = Guid.NewGuid();
        var companyId = db.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = originalId,
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        db.AuthConfigurations.Add(existingConfig);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "en", companyId);

        await service.UpsertAsync(command, CancellationToken.None);

        var updatedConfig = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.Id == originalId);

        Assert.NotNull(updatedConfig);

        Assert.Equal(originalId, updatedConfig.Id);
        Assert.Equal("en", updatedConfig.Value);
    }

    [Fact]
    public async Task Handle_MultipleUpdates_OnlyLastValuePersists()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;

        var command1 = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", companyId);

        var command2 = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "en", companyId);

        var command3 = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "es", companyId);

        await service.UpsertAsync(command1, CancellationToken.None);
        await service.UpsertAsync(command2, CancellationToken.None);
        await service.UpsertAsync(command3, CancellationToken.None);

        var configurations = await db.AuthConfigurations.Where(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId).ToListAsync(CancellationToken.None);

        Assert.Single(configurations);
        Assert.Equal("es", configurations[0].Value);
    }

    [Fact]
    public async Task Handle_WithEmptyValue_SavesEmptyString()
    {

        var command = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "", db.CurrentCompanyId ?? Guid.Empty);

        await service.UpsertAsync(command, CancellationToken.None);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal("", configuration.Value);
    }

    [Fact]
    public async Task Handle_WithLongValue_SavesSuccessfully()
    {

        var longValue = faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, longValue, db.CurrentCompanyId ?? Guid.Empty);

        await service.UpsertAsync(command, CancellationToken.None);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(longValue, configuration.Value);
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }
}
