namespace Fenicia.Auth.Domains.Configuration.DTOs;

public record GetConfigurationQuery(
    Guid UserId,
    Guid CompanyId);
