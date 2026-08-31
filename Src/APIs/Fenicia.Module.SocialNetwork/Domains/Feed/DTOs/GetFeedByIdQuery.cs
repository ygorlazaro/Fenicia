using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetFeedByIdQuery(
    [Required] Guid Id);
