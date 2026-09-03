namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetAllFeedQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null);