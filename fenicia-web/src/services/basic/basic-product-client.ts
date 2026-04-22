import { AxiosResponse } from 'axios';
import type { IPagination } from '../../types';
import type { AddProductCommand, AddProductResponse, DataSourceItem, GetAllProductResponse, GetProductByIdResponse, UpdateProductCommand, UpdateProductResponse } from '../../types/basic-types';
import { ApiClient } from '../api-client';
import { BasicCustomerClient } from './basic-customer-client';


export const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5083';


/**
 * Basic Product Client - Handles product CRUD operations
 */
export class BasicProductClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllProductResponse>> {
    const response = await this.getClient().get('/product', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }


  async getById(id: string): Promise<GetProductByIdResponse> {
    const response = await this.getClient().get(`/product/${id}`);
    return (response as AxiosResponse).data;
  }


  async create(product: AddProductCommand): Promise<AddProductResponse> {
    const response = await this.getClient().post('/product', product);
    return (response as AxiosResponse).data;
  }


  async update(id: string, product: UpdateProductCommand): Promise<UpdateProductResponse> {
    const response = await this.getClient().patch(`/product/${id}`, product);
    return (response as AxiosResponse).data;
  }


  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/product/${id}`);
  }

  async getProductCategories(): Promise<DataSourceItem[]> {
    const response = await this.getClient().get('/datasource/productcategory');
    return (response as AxiosResponse).data;
  }

  async getSuppliers(): Promise<DataSourceItem[]> {
    const response = await this.getClient().get('/datasource/supplier');
    return (response as AxiosResponse).data;
  }

}

export default BasicCustomerClient;
