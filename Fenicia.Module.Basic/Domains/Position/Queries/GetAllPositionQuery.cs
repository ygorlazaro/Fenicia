namespace Fenicia.Module.Basic.Domains.Position.Queries;

/// <summary>
///     Query to retrieve all positions with pagination.
/// </summary>
public record GetAllPositionQuery(int Page = 1, int PerPage = 10);