import { OrderStatus } from "./order-status";
import { PaymentMethod } from "./payment-method";


export type CreateOrderResponse = {
    id: string;
    orderNumber: string;
    userId: string;
    customerId: string;
    totalAmount: number;
    discountAmount: number;
    totalQuantity: number;
    saleDate: string;
    status: OrderStatus;
    paymentMethod: PaymentMethod;
    notes?: string;
    employeeId?: string;
};
