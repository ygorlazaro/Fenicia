using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public class GetBlockedResponse()
{
    public GetBlockedResponse(
        Guid id,
        Guid blockedProfileId,
        DateTime blockDate,
        string? reason)
        : this()
    {
        Id = id;
        BlockedProfileId = blockedProfileId;
        BlockDate = blockDate;
        Reason = reason;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid BlockedProfileId { get; init; }

    [Required]
    public DateTime BlockDate { get; init; }

    [MaxLength(200)]
    public string? Reason { get; init; }
}
