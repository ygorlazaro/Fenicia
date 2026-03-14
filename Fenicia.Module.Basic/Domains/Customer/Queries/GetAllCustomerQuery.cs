namespace Fenicia.Module.Basic.Domains.Customer.Queries;

/// <summary>
///     Query record for retrieving all customers with pagination.
/// </summary>
public record GetAllCustomerQuery(int Page = 1, int PerPage = 10);