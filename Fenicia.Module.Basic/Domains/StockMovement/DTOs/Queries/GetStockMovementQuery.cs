using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs.Queries;

public record GetStockMovementQuery(

    DateTime StartDate,

    DateTime EndDate,

    int Page = 1,

    int PerPage = 10)>;