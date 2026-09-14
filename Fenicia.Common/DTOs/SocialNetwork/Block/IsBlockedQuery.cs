using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public record IsBlockedQuery([Required] Guid BlockedProfileId);
