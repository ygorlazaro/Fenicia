import { AxiosResponse } from 'axios';
import { StockMovement } from '../../types/basic/stock-movement/stock-movement';
import { StockMovementDashboard } from '../../types/basic/stock-movement/stock-movement-dashboard';
import { ApiClient } from '../api-client';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5083';

/**
 * Stock Movement Client - Handles stock movement operations
 */
export class BasicStockMovementClient extends ApiClient {
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

export default BasicStockMovementClient;
