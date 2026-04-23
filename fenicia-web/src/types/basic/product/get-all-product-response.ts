
export type GetAllProductResponse = {
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
    categoryName: string;
    supplierId?: string;
    supplierName?: string;
    isActive: boolean;
};
