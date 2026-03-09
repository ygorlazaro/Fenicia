namespace Fenicia.Auth.Domains.Configuration.GetConfiguration;

public record GetConfigurationQuery(
    Guid UserId,
    Guid? CompanyId = null
);
