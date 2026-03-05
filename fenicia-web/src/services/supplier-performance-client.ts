import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface SupplierProductCount {
  supplierId: string;
  supplierName: string;
  productCount: number;
  totalStockValue: number;
  totalRevenue: number;
}

export interface ProductSupplierPrice {
  supplierId: string;
  supplierName: string;
  costPrice: number;
  salesPrice: number;
  profitMargin: number;
}

export interface SupplierCostComparison {
  productName: string;
  suppliers: ProductSupplierPrice[];
}

export interface SupplierStockMovement {
  movementId: string;
  productId: string;
  productName: string;
  quantity: number;
  price: number;
  date: string;
  movementType: string;
}

export interface SupplierSummary {
  totalSuppliers: number;
  totalProducts: number;
  totalStockValue: number;
  averageProductsPerSupplier: number;
}

export interface SupplierPerformance {
  productsPerSupplier: SupplierProductCount[];
  costComparison: SupplierCostComparison[];
  recentStockMovements: SupplierStockMovement[];
  summary: SupplierSummary;
}

export class SupplierPerformanceClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getPerformance(days?: number, topLimit?: number): Promise<SupplierPerformance> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topLimit !== undefined) params.topLimit = topLimit;
    
    const response = await this.getClient().get('/supplier/performance', { params });
    return (response as AxiosResponse).data;
  }
}

export default SupplierPerformanceClient;
