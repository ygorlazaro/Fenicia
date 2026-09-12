using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public record UnfollowCommand([Required] Guid TargetProfileId);
