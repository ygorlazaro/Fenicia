using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record AddFeedCommand(
    Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(512)] string Text,
    [Required] Guid UserId);