import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface OrderStatusCount {
  status: string;
  count: number;
  totalValue: number;
}

export interface SalesTrend {
  period: string;
  date: string;
  orderCount: number;
  totalValue: number;
  totalItems: number;
}

export interface TopCustomer {
  customerId: string;
  customerName: string;
  orderCount: number;
  totalSpent: number;
  totalItems: number;
}

export interface AverageOrderValue {
  averageValue: number;
  totalOrders: number;
  medianValue: number;
  minValue: number;
  maxValue: number;
}

export interface CancelledOrder {
  orderId: string;
  customerName: string;
  totalAmount: number;
  saleDate: string;
  totalItems: number;
  cancelledReason: string | null;
}

export interface OrderAnalytics {
  ordersByStatus: OrderStatusCount[];
  salesTrend: SalesTrend[];
  topCustomers: TopCustomer[];
  averageOrderValue: AverageOrderValue;
  cancelledOrders: CancelledOrder[];
}

export class OrderAnalyticsClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAnalytics(days?: number, topCustomersLimit?: number): Promise<OrderAnalytics> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topCustomersLimit !== undefined) params.topCustomersLimit = topCustomersLimit;
    
    const response = await this.getClient().get('/order/analytics', { params });
    return (response as AxiosResponse).data;
  }
}

export default OrderAnalyticsClient;
