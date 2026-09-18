namespace Fenicia.Common.DTOs.Basic.Product;

public class ProductPerformanceResponse
{
    public List<BestSellingProductResponse> BestSellingProducts { get; set; } = [];

    public List<WorstSellingProductResponse> WorstSellingProducts { get; set; } = [];

    public List<ProfitMarginResponse> ProfitMargins { get; set; } = [];

    public List<NeverSoldProductResponse> NeverSoldProducts { get; set; } = [];
}
