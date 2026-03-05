import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export interface OverstockProduct {
  productId: string;
  productName: string;
  categoryName: string;
  currentQuantity: number;
  recommendedQuantity: number;
  excessValue: number;
  costPrice: number;
}

export interface OverstockAlert {
  totalOverstockProducts: number;
  totalOverstockValue: number;
  products: OverstockProduct[];
}

export interface ZeroMovementProduct {
  productId: string;
  productName: string;
  categoryName: string;
  supplierName: string | null;
  currentStock: number;
  stockValue: number;
  lastMovementDate: string | null;
  daysWithoutMovement: number;
}

export interface StockValueByCategory {
  categoryId: string;
  categoryName: string;
  productCount: number;
  totalStockValue: number;
  percentage: number;
}

export interface InventoryHealthSummary {
  totalProducts: number;
  healthyProducts: number;
  overstockProducts: number;
  zeroMovementProducts: number;
  totalStockValue: number;
  overstockPercentage: number;
  zeroMovementPercentage: number;
}

export interface InventoryHealth {
  overstockAlert: OverstockAlert;
  zeroMovementProducts: ZeroMovementProduct[];
  stockValueByCategory: StockValueByCategory[];
  summary: InventoryHealthSummary;
}

export class InventoryHealthClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getInventoryHealth(zeroMovementDays?: number, overstockMultiplier?: number): Promise<InventoryHealth> {
    const params: any = {};
    if (zeroMovementDays !== undefined) params.zeroMovementDays = zeroMovementDays;
    if (overstockMultiplier !== undefined) params.overstockMultiplier = overstockMultiplier;
    
    const response = await this.getClient().get('/inventory/health', { params });
    return (response as AxiosResponse).data;
  }
}

export default InventoryHealthClient;
