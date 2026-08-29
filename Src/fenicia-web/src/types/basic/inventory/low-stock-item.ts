export interface LowStockItem {
    id: string;
    name: string;
    quantity: number;
    costPrice: number | null;
    salesPrice: number;
    categoryId: string;
    categoryName: string;
}
