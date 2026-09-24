using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Product;

/// <summary>
/// Request DTO for retrieving product performance metrics.
/// </summary>
/// <param name="days">The number of days to analyze (default: 90).</param>
/// <param name="topLimit">The maximum number of top/bottom items to return (default: 10).</param>
public record ProductPerformanceRequest(
    int Days = 90,
    int TopLimit = 10);