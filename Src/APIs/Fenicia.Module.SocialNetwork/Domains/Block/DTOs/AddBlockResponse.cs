namespace Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

public record AddBlockResponse(Guid Id, Guid UserId, Guid BlockedUserId, DateTime BlockDate, string? Reason, bool IsActive);
