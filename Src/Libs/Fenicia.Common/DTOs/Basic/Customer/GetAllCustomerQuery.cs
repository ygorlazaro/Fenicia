namespace Fenicia.Common.DTOs.Basic.Customer;

public record GetAllCustomerQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null)
{
    public Dictionary<string, string>? Filters { get; init; } = [];
}
