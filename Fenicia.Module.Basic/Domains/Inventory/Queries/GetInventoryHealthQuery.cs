namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

/// <summary>
///     Query record for generating inventory health analysis.
/// </summary>
public record GetInventoryHealthQuery(int ZeroMovementDays = 90, double OverstockMultiplier = 3.0);