export type AddProductCommand = {
    id: string;
    name: string;
    sku?: string;
    barcode?: string;
    description?: string;
    costPrice?: number;
    salesPrice: number;
    quantity: number;
    minStockLevel?: number;
    maxStockLevel?: number;
    imageUrl?: string;
    weight?: number;
    dimensions?: string;
    unitOfMeasure?: string;
    categoryId: string;
    supplierId?: string;
};
