using Fenicia.Auth.Domains.Configuration.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Configuration.Queries;

public record GetConfigurationQuery(

    Guid UserId,

    Guid CompanyId) : IRequest<List<GetConfigurationResponse>>;
