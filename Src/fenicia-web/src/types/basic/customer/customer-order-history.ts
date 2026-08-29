export interface CustomerOrderHistory {
    customerId: string;
    customerName: string;
    orderCount: number;
    totalSpent: number;
    totalItems: number;
    firstOrderDate: string;
    lastOrderDate: string;
    averageOrderValue: number;
}
