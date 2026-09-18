using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class GetStockMovementQuery
{
    public GetStockMovementQuery()
    {
    }

    public GetStockMovementQuery(
        DateTime? startDate,
        DateTime? endDate,
        StockMovementType? type = null,
        int page = 1,
        int perPage = 10,
        string? query = null)
    {
        StartDate = startDate;
        EndDate = endDate;
        Type = type;
        Page = page;
        PerPage = perPage;
        Query = query;
    }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public StockMovementType? Type { get; set; }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    public string? Query { get; set; }
}
