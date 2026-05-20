using Fenicia.Auth.Domains.Role.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Role.Queries;

public sealed record GetAdminRoleQuery : IRequest<GetAdminRoleResponse?>;
