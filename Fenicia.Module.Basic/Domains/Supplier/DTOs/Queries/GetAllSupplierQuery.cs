using Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Queries;

public record GetAllSupplierQuery(

    int Page = 1,

    int PerPage = 10);