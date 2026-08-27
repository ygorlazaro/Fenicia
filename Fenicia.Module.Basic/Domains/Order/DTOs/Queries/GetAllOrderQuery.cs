using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Order.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Order.DTOs.Queries;

public record GetAllOrderQuery(int Page = 1, int PerPage = 10);
