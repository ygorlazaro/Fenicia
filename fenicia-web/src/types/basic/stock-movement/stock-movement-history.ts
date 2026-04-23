
export interface StockMovementHistory {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  date: string;
  price: number;
  type: string;
  reason: string | null;
  customerName: string | null;
  supplierName: string | null;
  employeeName: string | null;
  orderId: string | null;
}
