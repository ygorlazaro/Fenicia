namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record GetAllCustomerQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);