namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Request DTO for retrieving inventory health data.
/// </summary>
/// <param name="zeroMovementDays">The number of days without movement to consider a product as zero movement (default: 90).</param>
public record InventoryHealthRequest(int ZeroMovementDays = 90);