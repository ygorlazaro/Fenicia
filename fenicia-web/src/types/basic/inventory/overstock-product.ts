
export interface OverstockProduct {
  productId: string;
  productName: string;
  categoryName: string;
  currentQuantity: number;
  recommendedQuantity: number;
  excessValue: number;
  costPrice: number;
}
