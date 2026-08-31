using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record DeleteNotificationCommand([Required] Guid Id);
