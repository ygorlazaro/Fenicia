namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryHealth;

public record GetInventoryHealthQuery(
    int ZeroMovementDays = 90,
    double OverstockMultiplier = 3.0); // Products with quantity > 3x average sales
