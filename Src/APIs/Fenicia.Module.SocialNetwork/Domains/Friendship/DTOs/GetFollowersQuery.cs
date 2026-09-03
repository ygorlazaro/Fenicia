namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowersQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null);