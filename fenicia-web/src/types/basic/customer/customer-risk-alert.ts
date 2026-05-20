export interface CustomerRiskAlert {
    customerId: string;
    customerName: string;
    previousOrderCount: number;
    lastOrderDate: string;
    daysSinceLastOrder: number;
    previousTotalSpent: number;
    riskLevel: string;
}
