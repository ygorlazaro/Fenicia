
export interface StockMovement {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  date: string | null;
  price: number | null;
  type: 'In' | 'Out';
  customerId: string | null;
  customerName: string | null;
  supplierId: string | null;
  supplierName: string | null;
  employeeId: string | null;
  employeeName: string | null;
  orderId: string | null;
  reason: string | null;
}
