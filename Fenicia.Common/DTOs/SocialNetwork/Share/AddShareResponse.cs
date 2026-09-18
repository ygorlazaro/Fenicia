using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public class AddShareResponse()
{
    public AddShareResponse(
        Guid id,
        Guid originalFeedId,
        string? text,
        Guid companyId,
        Guid profileId,
        DateTime shareDate,
        Guid shareFeedId)
        : this()
    {
        Id = id;
        OriginalFeedId = originalFeedId;
        Text = text;
        CompanyId = companyId;
        ProfileId = profileId;
        ShareDate = shareDate;
        ShareFeedId = shareFeedId;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid OriginalFeedId { get; init; }

    public string? Text { get; init; }

    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public DateTime ShareDate { get; init; }

    [Required]
    public Guid ShareFeedId { get; init; }
}
