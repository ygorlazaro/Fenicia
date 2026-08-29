import { DailySalesSummary } from "./daily-sales-summary";
import { FinancialAccountsReceivable } from "./financial-accounts-receivable";
import { FinancialProfitMarginTrend } from "./financial-profit-margin-trend";
import { KPISummary } from "./kpi-summary";
import { RevenueVsCost } from "./revenue-vs-cost";

export interface FinancialDashboard {
    kpi: KPISummary;
    revenueVsCost: RevenueVsCost[];
    profitMarginTrend: FinancialProfitMarginTrend[];
    accountsReceivable: FinancialAccountsReceivable;
    dailySales: DailySalesSummary;
}
