using Fenicia.Auth.Domains.Configuration.DTOs.Responses;

namespace Fenicia.Auth.Domains.Configuration.DTOs.Queries;

public record GetConfigurationQuery(
    Guid UserId,
    Guid CompanyId);
