using Bogus;

using Fenicia.Auth.Domains.Configuration.UpsertConfiguration;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

[TestFixture]
public class UpsertConfigurationHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new UpsertConfigurationHandler(this.context);
        this.faker = new Faker();
        this.testUserId = Guid.NewGuid();
        this.testCompanyId = Guid.NewGuid();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private UpsertConfigurationHandler handler = null!;
    private DefaultContext context = null!;
    private Faker faker = null!;
    private Guid testUserId;
    private Guid testCompanyId;

    [Test]
    public async Task Handle_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            Guid.NewGuid()
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.That(configuration, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(configuration.UserId, Is.EqualTo(this.testUserId));
            Assert.That(configuration.ConfigType, Is.EqualTo(ConfigType.Language));
            Assert.That(configuration.Value, Is.EqualTo("pt-BR"));
            Assert.That(configuration.CompanyId, Is.EqualTo(this.testCompanyId));
        }
    }

    [Test]
    public async Task Handle_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        // Arrange
        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
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
            existingConfig.CompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedConfig = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.That(updatedConfig, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedConfig.Value, Is.EqualTo("dark"));
            Assert.That(updatedConfig.Created, Is.EqualTo(existingConfig.Created), "Created date should not change");
        }
    }

    [Test]
    public async Task Handle_WithCompanyId_CreatesCompanyConfiguration()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.testCompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => 
                c.UserId == this.testUserId && 
                c.ConfigType == ConfigType.Language &&
                c.CompanyId == this.testCompanyId);

        Assert.That(configuration, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(configuration.CompanyId, Is.EqualTo(this.testCompanyId));
            Assert.That(configuration.Value, Is.EqualTo("pt-BR"));
        }
    }

    [Test]
    public async Task Handle_WithSameUserAndTypeButDifferentCompany_CreatesSeparateConfigurations()
    {
        // Arrange
        var userConfig = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            Guid.NewGuid()
        );

        var companyConfig = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "en",
            this.testCompanyId
        );

        // Act
        await this.handler.Handle(userConfig, CancellationToken.None);
        await this.handler.Handle(companyConfig, CancellationToken.None);

        // Assert
        var configurations = await this.context.AuthConfiguration
            .Where(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language)
            .ToListAsync(CancellationToken.None);

        Assert.That(configurations, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(configurations[0].CompanyId, Is.EqualTo(userConfig.CompanyId));
            Assert.That(configurations[0].Value, Is.EqualTo("dark"));
            Assert.That(configurations[1].CompanyId, Is.EqualTo(this.testCompanyId));
            Assert.That(configurations[1].Value, Is.EqualTo("light"));
        }
    }

    [Test]
    public async Task Handle_UpdatesConfigurationDoesNotChangeId()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = originalId,
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.context.AuthConfiguration.Add(existingConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "en",
            existingConfig.CompanyId
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedConfig = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.Id == originalId);

        Assert.That(updatedConfig, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedConfig.Id, Is.EqualTo(originalId));
            Assert.That(updatedConfig.Value, Is.EqualTo("disabled"));
        }
    }

    [Test]
    public async Task Handle_MultipleUpdates_OnlyLastValuePersists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
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
            .Where(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language)
            .ToListAsync(CancellationToken.None);

        Assert.That(configurations, Has.Count.EqualTo(1));
        Assert.That(configurations[0].Value, Is.EqualTo("auto"));
    }

    [Test]
    public async Task Handle_WithEmptyValue_SavesEmptyString()
    {
        // Arrange
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "",
            Guid.NewGuid()
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.That(configuration, Is.Not.Null);
        Assert.That(configuration.Value, Is.EqualTo(""));
    }

    [Test]
    public async Task Handle_WithLongValue_SavesSuccessfully()
    {
        // Arrange
        var longValue = this.faker.Lorem.Paragraphs(10);
        var command = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            longValue,
            Guid.NewGuid()
        );

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language);

        Assert.That(configuration, Is.Not.Null);
        Assert.That(configuration.Value, Is.EqualTo(longValue));
    }
}
