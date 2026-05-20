using Fenicia.Auth.Domains.Configuration.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Configuration.Queries;

/// <summary>
///     Query to retrieve user configurations.
/// </summary>
public record GetConfigurationQuery(
    /// <summary>
    /// The user ID to retrieve configurations for.
    /// </summary>
    Guid UserId,
    /// <summary>
    /// The company ID to filter configurations (now required).
    /// </summary>
    Guid CompanyId) : IRequest<List<GetConfigurationResponse>>;
