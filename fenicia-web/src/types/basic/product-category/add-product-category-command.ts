export type AddProductCategoryCommand = {
    id: string;
    name: string;
};

export type AddProductCategoryResponse = {
    id: string;
    name: string;
};

export type UpdateProductCategoryCommand = {
    id: string;
    name: string;
};

export type UpdateProductCategoryResponse = {
    id: string;
    name: string;
};

export type PaymentMethod = 'Cash' | 'CreditCard' | 'DebitCard' | 'BankTransfer' | 'Pix';

export type OrderStatus = 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';

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

export type OrderDetailResponse = {
    id: string;
    orderId: string;
    productId: string;
    price: number;
    discountAmount: number;
    quantity: number;
    subtotal: number;
};

export type DataSourceItem = {
    id: string;
    name: string;
};
