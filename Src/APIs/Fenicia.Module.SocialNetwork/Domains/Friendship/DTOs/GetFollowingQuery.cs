namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowingQuery(
    int Page = 1,
    int PerPage = 10);
