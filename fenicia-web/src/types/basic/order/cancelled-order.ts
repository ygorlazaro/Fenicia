
export interface CancelledOrder {
  orderId: string;
  customerName: string;
  totalAmount: number;
  saleDate: string;
  totalItems: number;
  cancelledReason: string | null;
}
