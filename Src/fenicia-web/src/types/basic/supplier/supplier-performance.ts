import { SupplierCostComparison } from "./supplier-cost-comparison";
import { SupplierProductCount } from "./supplier-product-count";
import { SupplierStockMovement } from "./supplier-stock-movement";
import { SupplierSummary } from "./supplier-summary";

export interface SupplierPerformance {
    productsPerSupplier: SupplierProductCount[];
    costComparison: SupplierCostComparison[];
    recentStockMovements: SupplierStockMovement[];
    summary: SupplierSummary;
}
