using Fenicia.Auth.Domains.Notification.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Notification.Commands;

public record AddNotificationCommand(
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl) : IRequest<AddNotificationResponse>;
