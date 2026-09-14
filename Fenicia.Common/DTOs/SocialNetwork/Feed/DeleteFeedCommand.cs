using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public record DeleteFeedCommand([Required] Guid Id);