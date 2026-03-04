import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

/**
 * Basic Customer Client - Handles customer CRUD operations
 */
export class BasicCustomerClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/customer', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/customer/${id}`);
    return (response as AxiosResponse).data;
  }

  async create(customer: any): Promise<any> {
    const response = await this.getClient().post('/customer', customer);
    return (response as AxiosResponse).data;
  }

  async update(id: string, customer: any): Promise<any> {
    const response = await this.getClient().patch(`/customer/${id}`, customer);
    return (response as AxiosResponse).data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/customer/${id}`);
  }

  async getStates(): Promise<any> {
    const response = await this.getClient().get('/state');
    return (response as AxiosResponse).data;
  }
}

/**
 * Basic Supplier Client - Handles supplier CRUD operations
 */
export class BasicSupplierClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/supplier', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/supplier/${id}`);
    return (response as AxiosResponse).data;
  }

  async create(supplier: any): Promise<any> {
    const response = await this.getClient().post('/supplier', supplier);
    return (response as AxiosResponse).data;
  }

  async update(id: string, supplier: any): Promise<any> {
    const response = await this.getClient().patch(`/supplier/${id}`, supplier);
    return (response as AxiosResponse).data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/supplier/${id}`);
  }

  async getStates(): Promise<any> {
    const response = await this.getClient().get('/state');
    return (response as AxiosResponse).data;
  }
}

/**
 * Basic Product Client - Handles product CRUD operations
 */
export class BasicProductClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/product', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/product/${id}`);
    return (response as AxiosResponse).data;
  }

  async create(product: any): Promise<any> {
    const response = await this.getClient().post('/product', product);
    return (response as AxiosResponse).data;
  }

  async update(id: string, product: any): Promise<any> {
    const response = await this.getClient().patch(`/product/${id}`, product);
    return (response as AxiosResponse).data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/product/${id}`);
  }
}

/**
 * Basic Product Category Client - Handles product category CRUD operations
 */
export class BasicProductCategoryClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/productcategory', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/productcategory/${id}`);
    return (response as AxiosResponse).data;
  }

  async create(category: any): Promise<any> {
    const response = await this.getClient().post('/productcategory', category);
    return (response as AxiosResponse).data;
  }

  async update(id: string, category: any): Promise<any> {
    const response = await this.getClient().patch(`/productcategory/${id}`, category);
    return (response as AxiosResponse).data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/productcategory/${id}`);
  }
}

export default BasicCustomerClient;
