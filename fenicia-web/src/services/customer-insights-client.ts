import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface CustomerSummary {
  totalCustomers: number;
  totalOrders: number;
  totalRevenue: number;
  averageOrderValue: number;
  averageCustomerLifetimeValue: number;
}

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

export interface CustomerRecentOrder {
  orderId: string;
  customerId: string;
  customerName: string;
  totalAmount: number;
  saleDate: string;
  status: string;
  totalItems: number;
}

export interface CustomerRiskAlert {
  customerId: string;
  customerName: string;
  previousOrderCount: number;
  lastOrderDate: string;
  daysSinceLastOrder: number;
  previousTotalSpent: number;
  riskLevel: string;
}

export interface CustomerInsights {
  summary: CustomerSummary;
  topCustomers: CustomerOrderHistory[];
  recentOrders: CustomerRecentOrder[];
  atRiskCustomers: CustomerRiskAlert[];
}

export class CustomerInsightsClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getInsights(days?: number, topLimit?: number, riskThresholdDays?: number): Promise<CustomerInsights> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topLimit !== undefined) params.topLimit = topLimit;
    if (riskThresholdDays !== undefined) params.riskThresholdDays = riskThresholdDays;
    
    const response = await this.getClient().get('/customer/insights', { params });
    return (response as AxiosResponse).data;
  }
}

export default CustomerInsightsClient;
