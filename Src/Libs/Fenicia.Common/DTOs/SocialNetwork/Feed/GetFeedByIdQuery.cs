using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public record GetFeedByIdQuery([Required] Guid Id);