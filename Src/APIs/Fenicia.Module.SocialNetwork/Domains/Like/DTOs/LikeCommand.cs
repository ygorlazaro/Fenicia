using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record LikeCommand(
    [Required] Guid FeedId);
