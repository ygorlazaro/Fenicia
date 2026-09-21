using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public class BlockRequest()
{
    public BlockRequest(Guid blockedProfileId)
        : this()
    {
        BlockedProfileId = blockedProfileId;
    }

    [Required]
    public Guid BlockedProfileId { get; set; }
}