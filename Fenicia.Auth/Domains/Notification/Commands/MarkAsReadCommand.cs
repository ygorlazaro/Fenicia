using MediatR;

namespace Fenicia.Auth.Domains.Notification.Commands;

public record MarkAsReadCommand(Guid Id) : IRequest<bool>;
