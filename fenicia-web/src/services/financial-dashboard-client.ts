import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface KPISummary {
  totalRevenue: number;
  totalCost: number;
  grossProfit: number;
  profitMargin: number;
  totalOrders: number;
  totalProducts: number;
  averageOrderValue: number;
  totalStockValue: number;
}

export interface RevenueVsCost {
  period: string;
  date: string;
  revenue: number;
  cost: number;
  profit: number;
}

export interface ProfitMarginTrend {
  period: string;
  date: string;
  marginPercentage: number;
  trend: string;
}

export interface AccountsReceivable {
  totalPending: number;
  pendingOrdersCount: number;
  totalApproved: number;
  approvedOrdersCount: number;
}

export interface DailySalesSummary {
  todayRevenue: number;
  todayOrders: number;
  weekRevenue: number;
  weekOrders: number;
  monthRevenue: number;
  monthOrders: number;
  previousMonthRevenue: number;
  growthPercentage: number;
}

export interface FinancialDashboard {
  kpi: KPISummary;
  revenueVsCost: RevenueVsCost[];
  profitMarginTrend: ProfitMarginTrend[];
  accountsReceivable: AccountsReceivable;
  dailySales: DailySalesSummary;
}

export class FinancialDashboardClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getFinancialDashboard(days?: number): Promise<FinancialDashboard> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    
    const response = await this.getClient().get('/dashboard/financial', { params });
    return (response as AxiosResponse).data;
  }
}

export default FinancialDashboardClient;
