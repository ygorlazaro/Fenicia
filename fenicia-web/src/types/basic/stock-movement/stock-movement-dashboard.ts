import { MonthlyInOut } from './monthly-in-out';
import { StockMovementHistory } from './stock-movement-history';
import { StockTurnover } from './stock-turnover';
import { TopMovedProduct } from './top-moved-product';


export interface StockMovementDashboard {
  history: StockMovementHistory[];
  monthlyInOut: MonthlyInOut[];
  topMovedProducts: TopMovedProduct[];
  turnoverRates: StockTurnover[];
}
