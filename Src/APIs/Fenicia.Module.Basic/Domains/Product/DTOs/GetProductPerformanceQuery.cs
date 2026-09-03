namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record GetProductPerformanceQuery(
    int Days = 90,
    int TopLimit = 10);