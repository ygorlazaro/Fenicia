using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public record GetUserProfileQuery([Required] Guid UserId);