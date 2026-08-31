namespace Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

public record GetBlockedQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null);
