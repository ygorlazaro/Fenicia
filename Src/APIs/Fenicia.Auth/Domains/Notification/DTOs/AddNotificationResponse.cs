using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record AddNotificationResponse([Required] Guid Id);