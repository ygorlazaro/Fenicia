using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public record UpdateNotificationResponse([Required] Guid Id);