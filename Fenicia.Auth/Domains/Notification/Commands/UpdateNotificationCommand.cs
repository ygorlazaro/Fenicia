using Fenicia.Auth.Domains.Notification.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Notification.Commands;

public record UpdateNotificationCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl,
    bool? Read) : IRequest<UpdateNotificationResponse?>;
