namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public record GetSharesByFeedQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null);