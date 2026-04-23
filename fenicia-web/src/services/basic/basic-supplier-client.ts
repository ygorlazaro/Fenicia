import { AxiosResponse } from 'axios';
import { IPagination } from '../../types';
import { AddSupplierCommand, AddSupplierResponse, GetAllSupplierResponse, GetSupplierByIdResponse, UpdateSupplierCommand, UpdateSupplierResponse } from '../../types/basic-types';
import { SupplierPerformance } from '../../types/basic/supplier/supplier-performance';
import { ApiClient } from '../api-client';
import { BASIC_API_BASE_URL } from './basic-product-client';


/**
 * Basic Supplier Client - Handles supplier CRUD operations
 */

export class BasicSupplierClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllSupplierResponse>> {
    const response = await this.getClient().get('/supplier', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }


  async getById(id: string): Promise<GetSupplierByIdResponse> {
    const response = await this.getClient().get(`/supplier/${id}`);
    return (response as AxiosResponse).data;
  }


  async create(supplier: AddSupplierCommand): Promise<AddSupplierResponse> {
    const response = await this.getClient().post('/supplier', supplier);
    return (response as AxiosResponse).data;
  }


  async update(id: string, supplier: UpdateSupplierCommand): Promise<UpdateSupplierResponse> {
    const response = await this.getClient().patch(`/supplier/${id}`, supplier);
    return (response as AxiosResponse).data;
  }


  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/supplier/${id}`);
  }

  async getPerformance(days?: number, topLimit?: number): Promise<SupplierPerformance> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topLimit !== undefined) params.topLimit = topLimit;

    const response = await this.getClient().get('/supplier/performance', { params });
    return (response as AxiosResponse).data;
  }
}
