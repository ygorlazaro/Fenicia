using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Subscription.DTOs;

public record GetUserProfileQuery([Required] Guid UserId);