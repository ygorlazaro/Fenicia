using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public record AddFeedCommand(
    Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(512)] string Text,
    [Required] Guid ProfileId,
    Guid? OriginalFeedId);
