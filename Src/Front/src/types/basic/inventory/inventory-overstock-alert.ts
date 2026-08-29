import { OverstockProduct } from "./overstock-product";

export interface InventoryOverstockAlert {
    totalOverstockProducts: number;
    totalOverstockValue: number;
    products: OverstockProduct[];
}
