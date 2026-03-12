namespace Fenicia.Module.Basic.Domains.Product.Queries;

public record GetProductPerformanceQuery(
    int Days = 90,
    int TopLimit = 10);
