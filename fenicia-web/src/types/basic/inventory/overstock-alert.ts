import { OverstockProduct } from './overstock-product';


export interface OverstockAlert {
  totalOverstockProducts: number;
  totalOverstockValue: number;
  products: OverstockProduct[];
}
