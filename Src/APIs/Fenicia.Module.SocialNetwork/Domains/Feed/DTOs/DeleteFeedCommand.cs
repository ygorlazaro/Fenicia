using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record DeleteFeedCommand(
    [Required] Guid Id);
