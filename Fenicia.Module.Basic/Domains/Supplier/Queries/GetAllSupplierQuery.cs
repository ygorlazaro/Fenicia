using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;
using Fenicia.Common;

namespace Fenicia.Module.Basic.Domains.Supplier.Queries;

public record GetAllSupplierQuery(

    int Page = 1,

    int PerPage = 10) : IRequest<Pagination<List<GetAllSupplierResponse>>>;