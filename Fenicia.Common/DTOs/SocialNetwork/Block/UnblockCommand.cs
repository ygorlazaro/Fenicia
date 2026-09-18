using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public class UnblockCommand()
{
    public UnblockCommand(Guid blockedProfileId)
        : this()
    {
        BlockedProfileId = blockedProfileId;
    }

    [Required]
    public Guid BlockedProfileId { get; set; }
}
