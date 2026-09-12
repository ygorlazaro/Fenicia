namespace Fenicia.Common.DTOs.Basic.Product;

public record GetProductPerformanceQuery(
    int Days = 90,
    int TopLimit = 10);