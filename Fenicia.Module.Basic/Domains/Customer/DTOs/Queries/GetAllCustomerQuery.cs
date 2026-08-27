using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Customer.DTOs.Queries;

public record GetAllCustomerQuery(int Page = 1, int PerPage = 10);
