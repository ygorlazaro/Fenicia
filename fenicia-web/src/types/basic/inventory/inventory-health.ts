import { InventoryHealthSummary } from './inventory-health-summary';
import { InventoryOverstockAlert } from './inventory-overstock-alert';
import { InventoryStockValueByCategory } from './inventory-stock-value-by-category';
import { ZeroMovementProduct } from './zero-movement-product';


export interface InventoryHealth {
  overstockAlert: InventoryOverstockAlert;
  zeroMovementProducts: ZeroMovementProduct[];
  stockValueByCategory: InventoryStockValueByCategory[];
  summary: InventoryHealthSummary;
}
