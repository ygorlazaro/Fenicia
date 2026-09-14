using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public record DeleteNotificationCommand([Required] Guid Id);