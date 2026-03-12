namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

public record GetInventoryHealthQuery(
    int ZeroMovementDays = 90,
    double OverstockMultiplier = 3.0); 
