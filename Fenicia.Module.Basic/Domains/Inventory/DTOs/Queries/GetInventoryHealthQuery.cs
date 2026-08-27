using Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;

public record GetInventoryHealthQuery(int ZeroMovementDays = 90, double OverstockMultiplier = 3.0);
