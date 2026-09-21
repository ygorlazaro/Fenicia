using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public class BlockResponse()
{
    public BlockResponse(
        Guid id,
        Guid profileId,
        Guid blockedProfileId,
        DateTime blockDate,
        string? reason,
        bool isActive)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        BlockedProfileId = blockedProfileId;
        BlockDate = blockDate;
        Reason = reason;
        IsActive = isActive;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid BlockedProfileId { get; init; }

    [Required]
    public DateTime BlockDate { get; init; }

    [MaxLength(200)]
    public string? Reason { get; init; }

    public bool IsActive { get; init; }
}