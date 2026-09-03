using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record UnlikeCommand([Required] Guid FeedId);