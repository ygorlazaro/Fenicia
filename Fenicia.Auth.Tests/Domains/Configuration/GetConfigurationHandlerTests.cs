using Fenicia.Auth.Domains.Configuration.GetConfiguration;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Configuration;

[TestFixture]
public class GetConfigurationHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetConfigurationHandler(this.context);
        this.testUserId = Guid.NewGuid();
        this.testCompanyId = Guid.NewGuid();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private GetConfigurationHandler handler = null!;
    private DefaultContext context = null!;
    private Guid testUserId;
    private Guid testCompanyId;

    [Test]
    public async Task Handle_WhenUserHasNoConfigurations_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetConfigurationQuery(this.testUserId, Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.Zero);
    }

    [Test]
    public async Task Handle_WhenUserHasConfigurations_ReturnsAllConfigurations()
    {
        // Arrange
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Timezone,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        this.context.AuthConfiguration.AddRange(config1, config2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId, config1.CompanyId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].ConfigType, Is.EqualTo(ConfigType.Language));
            Assert.That(result[1].ConfigType, Is.EqualTo(ConfigType.Language));
        }
    }

    [Test]
    public async Task Handle_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        // Arrange
        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "en"
        };

        var companyConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = this.testCompanyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.context.AuthConfiguration.AddRange(userConfig, companyConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId, this.testCompanyId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].CompanyId, Is.EqualTo(this.testCompanyId));
    }

    [Test]
    public async Task Handle_WithNonExistentCompanyId_ReturnsEmptyList()
    {
        // Arrange
        var config = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = this.testCompanyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.context.AuthConfiguration.Add(config);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var nonExistentCompanyId = Guid.NewGuid();
        var query = new GetConfigurationQuery(this.testUserId, nonExistentCompanyId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.Zero);
    }

    [Test]
    public async Task Handle_ConfigurationsAreOrderedByConfigType()
    {
        // Arrange
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Timezone,
            Value = "GMT-3"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config3 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        this.context.AuthConfiguration.AddRange(config1, config2, config3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId, config3.CompanyId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].ConfigType, Is.EqualTo(ConfigType.Timezone));
            Assert.That(result[1].ConfigType, Is.EqualTo(ConfigType.Language));
            Assert.That(result[2].ConfigType, Is.EqualTo(ConfigType.Language));
        }
    }

    [Test]
    public async Task Handle_ResponseContainsCorrectData()
    {
        // Arrange
        var configId = Guid.NewGuid();
        var config = new ConfigurationModel
        {
            Id = configId,
            UserId = this.testUserId,
            CompanyId = this.testCompanyId,
            ConfigType = ConfigType.Language,
            Value = "pt-bR"
        };

        this.context.AuthConfiguration.Add(config);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetConfigurationQuery(this.testUserId, this.testCompanyId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(configId));
            Assert.That(result[0].UserId, Is.EqualTo(this.testUserId));
            Assert.That(result[0].CompanyId, Is.EqualTo(this.testCompanyId));
            Assert.That(result[0].ConfigType, Is.EqualTo(ConfigType.Language));
            Assert.That(result[0].Value, Is.EqualTo("pt-BR"));
        }
    }
}
