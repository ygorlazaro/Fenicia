namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record GetInventoryHealthQuery(int ZeroMovementDays = 90, double OverstockMultiplier = 3.0);