import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface EmployeePerformanceSummary {
  totalEmployees: number;
  activeEmployees: number;
  totalSales: number;
  totalOrders: number;
  averageSalesPerEmployee: number;
  averageOrdersPerEmployee: number;
}

export interface EmployeeSales {
  employeeId: string;
  employeeName: string;
  positionName: string;
  totalSales: number;
  totalOrders: number;
  averageOrderValue: number;
  rank: number;
}

export interface EmployeeOrderCount {
  employeeId: string;
  employeeName: string;
  positionName: string;
  orderCount: number;
  totalValue: number;
  firstOrderDate: string;
  lastOrderDate: string;
}

export interface TopPerformer {
  employeeId: string;
  employeeName: string;
  positionName: string;
  totalSales: number;
  totalOrders: number;
  performanceLevel: string;
}

export interface EmployeePerformance {
  summary: EmployeePerformanceSummary;
  salesByEmployee: EmployeeSales[];
  ordersByEmployee: EmployeeOrderCount[];
  topPerformers: TopPerformer[];
}

export class EmployeePerformanceClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getPerformance(days?: number, topLimit?: number): Promise<EmployeePerformance> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topLimit !== undefined) params.topLimit = topLimit;
    
    const response = await this.getClient().get('/employee/performance', { params });
    return (response as AxiosResponse).data;
  }
}

export default EmployeePerformanceClient;
