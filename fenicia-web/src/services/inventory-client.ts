import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

/**
 * Inventory Client - Handles inventory dashboard operations
 */
export class InventoryClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getDashboard(): Promise<any> {
    const response = await this.getClient().get('/inventory/dashboard');
    return (response as AxiosResponse).data;
  }

  async getInventory(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/inventory', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getByProduct(productId: string): Promise<any> {
    const response = await this.getClient().get(`/inventory/product/${productId}`);
    return (response as AxiosResponse).data;
  }

  async getByCategory(categoryId: string): Promise<any> {
    const response = await this.getClient().get(`/inventory/category/${categoryId}`);
    return (response as AxiosResponse).data;
  }
}

export default InventoryClient;
