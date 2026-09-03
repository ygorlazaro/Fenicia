using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record GetNotificationByIdQuery([Required] Guid Id);