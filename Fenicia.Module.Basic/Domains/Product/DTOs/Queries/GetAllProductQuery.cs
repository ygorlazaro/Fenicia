using Fenicia.Module.Basic.Domains.Product.DTOs.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.Product.DTOs.Queries;

public record GetAllProductQuery(

    int Page = 1,

    int PerPage = 10);