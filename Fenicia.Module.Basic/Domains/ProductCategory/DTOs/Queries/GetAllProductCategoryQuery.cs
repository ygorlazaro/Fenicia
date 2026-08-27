using Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Queries;

public record GetAllProductCategoryQuery(

    int Page = 1,

    int PerPage = 10);