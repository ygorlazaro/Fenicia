import { OrderStatus } from "./order-status";
import { PaymentMethod } from "./payment-method";

export type GetAllOrderResponse = {
    id: string;
    orderNumber: string;
    userId: string;
    customerId: string;
    customerName: string;
    totalAmount: number;
    discountAmount: number;
    totalQuantity: number;
    saleDate: string;
    status: OrderStatus;
    paymentMethod: PaymentMethod;
    totalItems: number;
    employeeId?: string;
    employeeName?: string;
};
