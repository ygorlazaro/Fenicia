using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public class FeedResponse()
{
    public FeedResponse(
        Guid id,
        DateTime date,
        string text,
        Guid profileId,
        Guid companyId,
        int totalLikes,
        int totalComments,
        int totalShares,
        Guid? originalFeedId,
        string? authorUserName,
        string? authorImageUrl)
        : this()
    {
        Id = id;
        Date = date;
        Text = text;
        ProfileId = profileId;
        CompanyId = companyId;
        TotalLikes = totalLikes;
        TotalComments = totalComments;
        TotalShares = totalShares;
        OriginalFeedId = originalFeedId;
        AuthorUserName = authorUserName;
        AuthorImageUrl = authorImageUrl;
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

    public int TotalLikes { get; init; }
    public int TotalComments { get; init; }
    public int TotalShares { get; init; }

    public Guid? OriginalFeedId { get; init; }

    [MaxLength(64)]
    public string? AuthorUserName { get; init; }

    [MaxLength(200)]
    public string? AuthorImageUrl { get; init; }
}