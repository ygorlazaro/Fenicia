export interface ZeroMovementProduct {
    productId: string;
    productName: string;
    categoryName: string;
    supplierName: string | null;
    currentStock: number;
    stockValue: number;
    lastMovementDate: string | null;
    daysWithoutMovement: number;
}
