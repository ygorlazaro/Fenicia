
export interface InventoryHealthSummary {
  totalProducts: number;
  healthyProducts: number;
  overstockProducts: number;
  zeroMovementProducts: number;
  totalStockValue: number;
  overstockPercentage: number;
  zeroMovementPercentage: number;
}
