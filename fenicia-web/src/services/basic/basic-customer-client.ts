import { AxiosResponse } from 'axios';
import { IPagination } from '../../types';
import { AddCustomerCommand } from "../../types/basic/customer/add-customer-command";
import { AddCustomerResponse } from "../../types/basic/customer/add-customer-response";
import { CustomerInsights } from '../../types/basic/customer/customer-insights';
import { GetAllCustomerResponse } from "../../types/basic/customer/get-all-customer-response";
import { GetCustomerByIdResponse } from "../../types/basic/customer/get-customer-by-id-response";
import { UpdateCustomerCommand } from "../../types/basic/customer/update-customer-command";
import { UpdateCustomerResponse } from "../../types/basic/customer/update-customer-response";
import { ApiClient } from '../api-client';
import { BASIC_API_BASE_URL } from './basic-product-client';

/**
 * Basic Customer Client - Handles customer CRUD operations
 */

class BasicCustomerClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllCustomerResponse>> {
    const response = await this.getClient().get('/customer', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getById(id: string): Promise<GetCustomerByIdResponse> {
    const response = await this.getClient().get(`/customer/${id}`);
    return (response as AxiosResponse).data;
  }

  async create(customer: AddCustomerCommand): Promise<AddCustomerResponse> {
    const response = await this.getClient().post('/customer', customer);
    return (response as AxiosResponse).data;
  }

  async update(id: string, customer: UpdateCustomerCommand): Promise<UpdateCustomerResponse> {
    const response = await this.getClient().patch(`/customer/${id}`, customer);
    return (response as AxiosResponse).data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/customer/${id}`);
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

export default BasicCustomerClient;
