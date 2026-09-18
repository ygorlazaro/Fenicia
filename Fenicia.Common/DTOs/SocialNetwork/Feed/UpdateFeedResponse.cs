using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public class UpdateFeedResponse()
{
    public UpdateFeedResponse(
        Guid id,
        DateTime date,
        string text,
        Guid profileId,
        Guid companyId,
        Guid? originalFeedId)
        : this()
    {
        Id = id;
        Date = date;
        Text = text;
        ProfileId = profileId;
        CompanyId = companyId;
        OriginalFeedId = originalFeedId;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public DateTime Date { get; init; }

    [Required]
    [MaxLength(200)]
    public string Text { get; init; } = string.Empty;

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid CompanyId { get; init; }

    public Guid? OriginalFeedId { get; init; }
}
