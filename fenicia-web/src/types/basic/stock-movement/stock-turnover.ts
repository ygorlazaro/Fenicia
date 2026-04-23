
export interface StockTurnover {
  productId: string;
  productName: string;
  categoryName: string;
  currentStock: number;
  totalSold: number;
  turnoverRate: number;
  turnoverClassification: string;
}
