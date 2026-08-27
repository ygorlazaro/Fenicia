using Fenicia.Auth.Domains.Configuration.Responses;

namespace Fenicia.Auth.Domains.Configuration.Queries;

public record GetConfigurationQuery(
    Guid UserId,
    Guid CompanyId);
