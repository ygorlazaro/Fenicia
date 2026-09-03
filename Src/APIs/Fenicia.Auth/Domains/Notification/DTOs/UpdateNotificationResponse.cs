using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record UpdateNotificationResponse([Required] Guid Id);