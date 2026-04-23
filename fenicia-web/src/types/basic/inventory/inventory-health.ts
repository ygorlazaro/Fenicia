import { InventoryHealthSummary } from './inventory-health-summary';
import { OverstockAlert } from './overstock-alert';
import { StockValueByCategory } from './stock-value-by-category';
import { ZeroMovementProduct } from './zero-movement-product';


export interface InventoryHealth {
  overstockAlert: OverstockAlert;
  zeroMovementProducts: ZeroMovementProduct[];
  stockValueByCategory: StockValueByCategory[];
  summary: InventoryHealthSummary;
}
