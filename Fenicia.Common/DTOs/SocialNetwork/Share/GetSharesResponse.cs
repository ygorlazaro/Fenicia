using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public class GetSharesResponse()
{
    public GetSharesResponse(
        Guid id,
        Guid originalFeedId,
        string? text,
        Guid companyId,
        Guid profileId,
        DateTime shareDate)
        : this()
    {
        Id = id;
        OriginalFeedId = originalFeedId;
        Text = text;
        CompanyId = companyId;
        ProfileId = profileId;
        ShareDate = shareDate;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid OriginalFeedId { get; init; }

    [MaxLength(200)]
    public string? Text { get; init; }

    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public DateTime ShareDate { get; init; }
}
