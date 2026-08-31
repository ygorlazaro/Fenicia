namespace Fenicia.Common;

public record AdvancedQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);
