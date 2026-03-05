import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface BestSellingProduct {
  productId: string;
  productName: string;
  categoryName: string;
  totalQuantitySold: number;
  totalRevenue: number;
  orderCount: number;
  averagePrice: number;
}

export interface WorstSellingProduct {
  productId: string;
  productName: string;
  categoryName: string;
  totalQuantitySold: number;
  totalRevenue: number;
  orderCount: number;
  currentStock: number;
  costValue: number;
}

export interface ProfitMargin {
  productId: string;
  productName: string;
  categoryName: string;
  costPrice: number;
  salesPrice: number;
  profitMargin: number;
  marginClassification: string;
}

export interface NeverSoldProduct {
  productId: string;
  productName: string;
  categoryName: string;
  supplierName: string | null;
  currentStock: number;
  costValue: number;
  lastStockMovement: string | null;
}

export interface ProductPerformance {
  bestSellingProducts: BestSellingProduct[];
  worstSellingProducts: WorstSellingProduct[];
  profitMargins: ProfitMargin[];
  neverSoldProducts: NeverSoldProduct[];
}

export class ProductPerformanceClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getPerformance(days?: number, topLimit?: number): Promise<ProductPerformance> {
    const params: any = {};
    if (days !== undefined) params.days = days;
    if (topLimit !== undefined) params.topLimit = topLimit;
    
    const response = await this.getClient().get('/product/performance', { params });
    return (response as AxiosResponse).data;
  }
}

export default ProductPerformanceClient;
