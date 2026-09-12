using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public record UpdateFeedCommand(
    [Required] Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(200)] string Text);