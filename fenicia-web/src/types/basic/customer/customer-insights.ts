import { CustomerOrderHistory } from "./customer-order-history";
import { CustomerRecentOrder } from "./customer-recent-order";
import { CustomerRiskAlert } from "./customer-risk-alert";
import { CustomerSummary } from "./customer-summary";

export interface CustomerInsights {
    summary: CustomerSummary;
    topCustomers: CustomerOrderHistory[];
    recentOrders: CustomerRecentOrder[];
    atRiskCustomers: CustomerRiskAlert[];
}
