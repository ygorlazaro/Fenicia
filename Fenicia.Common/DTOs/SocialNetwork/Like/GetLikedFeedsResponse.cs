using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class GetLikedFeedsResponse()
{
    public GetLikedFeedsResponse(
        Guid id,
        DateTime date,
        string text,
        Guid profileId,
        Guid companyId,
        int totalLikes,
        int totalComments,
        int totalShares)
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
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public DateTime Date { get; init; }

    [Required]
    [MaxLength(512)]
    public string Text { get; init; } = string.Empty;

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid CompanyId { get; init; }

    public int TotalLikes { get; init; }

    public int TotalComments { get; init; }

    public int TotalShares { get; init; }
}
