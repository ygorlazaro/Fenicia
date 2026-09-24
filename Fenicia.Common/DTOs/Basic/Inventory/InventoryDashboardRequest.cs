namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Request DTO for retrieving inventory dashboard data.
/// </summary>
/// <param name="days">The period in days for analysis (default: 90).</param>
public record InventoryDashboardRequest(int Days = 90);