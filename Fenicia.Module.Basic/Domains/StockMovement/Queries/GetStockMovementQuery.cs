using MediatR;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

namespace Fenicia.Module.Basic.Domains.StockMovement.Queries;

public record GetStockMovementQuery(

    DateTime StartDate,

    DateTime EndDate,

    int Page = 1,

    int PerPage = 10) : IRequest<List<GetStockMovementResponse>>;