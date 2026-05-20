import { OrderDetailResponse } from "./order-detail-response";
import { OrderStatus } from "./order-status";
import { PaymentMethod } from "./payment-method";

export type GetOrderByIdResponse = {
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
    notes?: string;
    details: OrderDetailResponse[];
    employeeId?: string;
};
