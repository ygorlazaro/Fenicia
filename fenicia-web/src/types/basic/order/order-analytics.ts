import { AverageOrderValue } from './average-order-value';
import { CancelledOrder } from './cancelled-order';
import { OrderStatusCount } from './order-status-count';
import { SalesTrend } from './sales-trend';
import { TopCustomer } from './top-customer';


export interface OrderAnalytics {
  ordersByStatus: OrderStatusCount[];
  salesTrend: SalesTrend[];
  topCustomers: TopCustomer[];
  averageOrderValue: AverageOrderValue;
  cancelledOrders: CancelledOrder[];
}
