using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public record FollowCommand([Required] Guid TargetProfileId);
