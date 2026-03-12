namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record ProductPerformanceResponse
{
    public List<BestSellingProductResponse> BestSellingProducts { get; set; } = [];
    public List<WorstSellingProductResponse> WorstSellingProducts { get; set; } = [];
    public List<ProfitMarginResponse> ProfitMargins { get; set; } = [];
    public List<NeverSoldProductResponse> NeverSoldProducts { get; set; } = [];
}
