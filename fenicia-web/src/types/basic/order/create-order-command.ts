import { PaymentMethod } from "./payment-method";


export type CreateOrderCommand = {
    customerId: string;
    saleDate: string;
    details: Array<{
        productId: string;
        quantity: number;
        price: number;
    }>;
    discountAmount?: number;
    paymentMethod: PaymentMethod;
    notes?: string;
    employeeId?: string;
    status: string;
};
