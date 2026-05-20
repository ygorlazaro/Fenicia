import { BestSellingProduct } from "./best-selling-product";
import { NeverSoldProduct } from "./never-sold-product";
import { ProfitMargin } from "./profit-margin";
import { WorstSellingProduct } from "./worst-selling-product";

export interface ProductPerformance {
    bestSellingProducts: BestSellingProduct[];
    worstSellingProducts: WorstSellingProduct[];
    profitMargins: ProfitMargin[];
    neverSoldProducts: NeverSoldProduct[];
}
