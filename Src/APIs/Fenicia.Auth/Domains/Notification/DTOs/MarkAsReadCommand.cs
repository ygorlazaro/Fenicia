using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record MarkAsReadCommand([Required] Guid Id);
