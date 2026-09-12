namespace Fenicia.Common.DTOs.Basic.Supplier;

public record GetAllSupplierQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null)
{
    public Dictionary<string, string>? Filters { get; init; } = [];
}
