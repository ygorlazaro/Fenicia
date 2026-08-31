namespace Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

public record GetBlockedResponse(Guid Id, Guid BlockedUserId, DateTime BlockDate, string? Reason);
