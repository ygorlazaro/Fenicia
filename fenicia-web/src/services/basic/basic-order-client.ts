import { AxiosResponse } from 'axios';
import { IPagination } from '../../types';
import { CreateOrderCommand, CreateOrderResponse, GetAllOrderResponse, GetOrderByIdResponse, OrderDetailResponse } from '../../types/basic-types';
import { OrderAnalytics } from '../../types/basic/order/order-analytics';
import { ApiClient } from '../api-client';
import { BASIC_API_BASE_URL } from './basic-product-client';


/**
 * Basic Order Client - Handles order CRUD operations
 */

export class BasicOrderClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllOrderResponse>> {
    const response = await this.getClient().get('/order', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }


  async getById(id: string): Promise<GetOrderByIdResponse> {
    const response = await this.getClient().get(`/order/${id}`);
    return (response as AxiosResponse).data;
  }


  async create(order: CreateOrderCommand): Promise<CreateOrderResponse> {
    const response = await this.getClient().post('/order', order);
    return (response as AxiosResponse).data;
  }


  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/order/${id}`);
  }

  async getDetails(id: string): Promise<OrderDetailResponse[]> {
    const response = await this.getClient().get(`/order/${id}/detail`);
    return (response as AxiosResponse).data;
  }

  async getAnalytics(days?: number, topCustomersLimit?: number): Promise<OrderAnalytics> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topCustomersLimit !== undefined) params.topCustomersLimit = topCustomersLimit;

    const response = await this.getClient().get('/order/analytics', { params });
    return (response as AxiosResponse).data;
  }
}
