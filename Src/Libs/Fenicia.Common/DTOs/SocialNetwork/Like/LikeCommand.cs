using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public record LikeCommand([Required] Guid FeedId);