using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public record IsFollowingQuery([Required] Guid TargetProfileId);
