using MediatR;

namespace Fenicia.Auth.Domains.Notification.Commands;

public record DeleteNotificationCommand(Guid Id) : IRequest;
