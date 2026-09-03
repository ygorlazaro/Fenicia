using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record UpdateFeedCommand(
    [Required] Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(200)] string Text);