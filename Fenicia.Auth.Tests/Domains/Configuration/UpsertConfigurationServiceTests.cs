using Bogus;

using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class UpsertConfigurationServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ConfigurationService _service;
    private readonly Guid _testUserId;

    public UpsertConfigurationServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _service = new ConfigurationService(_db);
        _faker = new Faker();
        _testUserId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR", _db.CurrentCompanyId ?? Guid.Empty);

        await _service.UpsertAsync(command, CancellationToken.None);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);

        Assert.Equal(_testUserId, configuration.UserId);
        Assert.Equal(ConfigType.Language, configuration.ConfigType);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(_db.CurrentCompanyId, configuration.CompanyId);
    }

    [Fact]
    public async Task Handle_WithSameUserAndTypeButDifferentCompany_CreatesSeparateConfigurations()
    {
        var config1 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR", _db.CurrentCompanyId ?? Guid.NewGuid());

        var config2 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Timezone, "dark", _db.CurrentCompanyId ?? Guid.NewGuid());

        await _service.UpsertAsync(config1, CancellationToken.None);
        await _service.UpsertAsync(config2, CancellationToken.None);

        var configurations = await _db.AuthConfigurations.Where(c => c.UserId == _testUserId).ToListAsync(CancellationToken.None);

        Assert.Equal(2, configurations.Count);
    }

    [Fact]
    public async Task Handle_UpdatesConfigurationDoesNotChangeId()
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

        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "en", companyId);

        await _service.UpsertAsync(command, CancellationToken.None);

        var updatedConfig = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.Id == originalId);

        Assert.NotNull(updatedConfig);

        Assert.Equal(originalId, updatedConfig.Id);
        Assert.Equal("en", updatedConfig.Value);
    }

    [Fact]
    public async Task Handle_MultipleUpdates_OnlyLastValuePersists()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;

        var command1 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR", companyId);

        var command2 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "en", companyId);

        var command3 = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "es", companyId);

        await _service.UpsertAsync(command1, CancellationToken.None);
        await _service.UpsertAsync(command2, CancellationToken.None);
        await _service.UpsertAsync(command3, CancellationToken.None);

        var configurations = await _db.AuthConfigurations.Where(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId).ToListAsync(CancellationToken.None);

        Assert.Single(configurations);
        Assert.Equal("es", configurations[0].Value);
    }

    [Fact]
    public async Task Handle_WithEmptyValue_SavesEmptyString()
    {
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, string.Empty, _db.CurrentCompanyId ?? Guid.Empty);

        await _service.UpsertAsync(command, CancellationToken.None);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(string.Empty, configuration.Value);
    }

    [Fact]
    public async Task Handle_WithLongValue_SavesSuccessfully()
    {
        var longValue = _faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, longValue, _db.CurrentCompanyId ?? Guid.Empty);

        await _service.UpsertAsync(command, CancellationToken.None);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language);

        Assert.NotNull(configuration);
        Assert.Equal(longValue, configuration.Value);
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }
}
