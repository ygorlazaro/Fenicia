using Fenicia.Auth.Domains.Notification.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Notification.Queries;

public record GetNotificationByIdQuery(Guid Id) : IRequest<GetNotificationByIdResponse?>;
