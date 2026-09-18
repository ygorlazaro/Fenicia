using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public class BlockCommand()
{
    public BlockCommand(Guid blockedProfileId)
        : this()
    {
        BlockedProfileId = blockedProfileId;
    }

    [Required]
    public Guid BlockedProfileId { get; set; }
}
