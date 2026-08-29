import { AverageOrderValue } from "./average-order-value";
import { OrderCancelledOrder } from "./order-cancelled-order";
import { OrderStatusCount } from "./order-status-count";
import { OrderTopCustomer } from "./order-top-customer";
import { SalesTrend } from "./sales-trend";

export interface OrderAnalytics {
    ordersByStatus: OrderStatusCount[];
    salesTrend: SalesTrend[];
    topCustomers: OrderTopCustomer[];
    averageOrderValue: AverageOrderValue;
    cancelledOrders: OrderCancelledOrder[];
}
