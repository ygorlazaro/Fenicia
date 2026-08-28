namespace Fenicia.Auth.Domains.Configuration.DTOs.Queries;

public record GetConfigurationQuery(
    Guid UserId,
    Guid CompanyId);
