using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public class IsBlockedQuery()
{
    public IsBlockedQuery(Guid blockedProfileId)
        : this()
    {
        BlockedProfileId = blockedProfileId;
    }

    [Required]
    public Guid BlockedProfileId { get; set; }
}
