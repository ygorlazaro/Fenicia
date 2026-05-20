export interface NeverSoldProduct {
    productId: string;
    productName: string;
    categoryName: string;
    supplierName: string | null;
    currentStock: number;
    costValue: number;
    lastStockMovement: string | null;
}
