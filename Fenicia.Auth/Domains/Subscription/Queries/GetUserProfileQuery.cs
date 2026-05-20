using Fenicia.Auth.Domains.Subscription.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Subscription.Queries;

public record GetUserProfileQuery(Guid UserId) : IRequest<GetUserProfileResponse?>;
