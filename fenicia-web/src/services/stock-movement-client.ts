import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface StockMovement {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  date: string | null;
  price: number | null;
  type: 'In' | 'Out';
  customerId: string | null;
  customerName: string | null;
  supplierId: string | null;
  supplierName: string | null;
  employeeId: string | null;
  employeeName: string | null;
  orderId: string | null;
  reason: string | null;
}

export interface StockMovementHistory {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  date: string;
  price: number;
  type: string;
  reason: string | null;
  customerName: string | null;
  supplierName: string | null;
  employeeName: string | null;
  orderId: string | null;
}

export interface MonthlyInOut {
  month: string;
  totalIn: number;
  totalOut: number;
  totalInValue: number;
  totalOutValue: number;
}

export interface TopMovedProduct {
  productId: string;
  productName: string;
  categoryName: string;
  totalMoved: number;
  totalValue: number;
  movementCount: number;
}

export interface StockTurnover {
  productId: string;
  productName: string;
  categoryName: string;
  currentStock: number;
  totalSold: number;
  turnoverRate: number;
  turnoverClassification: string;
}

export interface StockMovementDashboard {
  history: StockMovementHistory[];
  monthlyInOut: MonthlyInOut[];
  topMovedProducts: TopMovedProduct[];
  turnoverRates: StockTurnover[];
}

/**
 * Stock Movement Client - Handles stock movement operations
 */
export class StockMovementClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getMovements(startDate: string, endDate: string, page: number = 1, perPage: number = 10): Promise<StockMovement[]> {
    const response = await this.getClient().get('/stockmovement', {
      params: { startDate, endDate, page, perPage }
    });
    return (response as AxiosResponse).data;
  }

  async getDashboard(days?: number, topLimit?: number): Promise<StockMovementDashboard> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topLimit !== undefined) params.topLimit = topLimit;
    
    const response = await this.getClient().get('/stockmovement/dashboard', { params });
    return (response as AxiosResponse).data;
  }

  async create(movement: Partial<StockMovement>): Promise<StockMovement> {
    const response = await this.getClient().post('/stockmovement', movement);
    return (response as AxiosResponse).data;
  }

  async update(id: string, movement: Partial<StockMovement>): Promise<StockMovement> {
    const response = await this.getClient().patch(`/stockmovement/${id}`, movement);
    return (response as AxiosResponse).data;
  }
}

export default StockMovementClient;
