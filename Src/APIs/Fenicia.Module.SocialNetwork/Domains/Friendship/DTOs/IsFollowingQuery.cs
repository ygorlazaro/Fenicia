using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record IsFollowingQuery([Required] Guid TargetProfileId);
