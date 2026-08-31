namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record AddShareResponse(Guid Id, Guid OriginalFeedId, string? Text, Guid CompanyId, Guid UserId, DateTime ShareDate);
