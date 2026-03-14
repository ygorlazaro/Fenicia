namespace Fenicia.Module.Basic.Domains.Order.Queries;

/// <summary>
/// Query to retrieve a paginated list of orders.
/// </summary>
public record GetAllOrderQuery(int Page = 1, int PerPage = 10);
