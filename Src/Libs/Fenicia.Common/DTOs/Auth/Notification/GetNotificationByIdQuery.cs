using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public record GetNotificationByIdQuery([Required] Guid Id);