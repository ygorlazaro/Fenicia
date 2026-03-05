namespace Fenicia.Module.Basic.Domains.Product.GetProductPerformance;

public record GetProductPerformanceQuery(
    int Days = 90,
    int TopLimit = 10);
