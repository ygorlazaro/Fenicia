import { CategoryBreakdown } from './category-breakdown';
import { LowStockItem } from './low-stock-item';
import { SupplierBreakdown } from './supplier-breakdown';

export interface DashboardData {
    lowStockItems: LowStockItem[];
    totalCustomers: number;
    totalEmployees: number;
    totalCostValue: number;
    totalSalesValue: number;
    totalQuantity: number;
    profitPotential: number;
    categoryBreakdown: CategoryBreakdown[];
    supplierBreakdown: SupplierBreakdown[];
}
