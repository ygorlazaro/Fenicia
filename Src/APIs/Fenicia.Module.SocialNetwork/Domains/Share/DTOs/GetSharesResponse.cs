namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record GetSharesResponse(Guid Id, Guid OriginalFeedId, string? Text, Guid CompanyId, Guid UserId, DateTime ShareDate);
