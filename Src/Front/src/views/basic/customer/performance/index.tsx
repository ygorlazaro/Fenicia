import { CSpinner } from "@coreui/react";
import { useTranslation } from "react-i18next";
import TimeRangeSelector from "../../../../components/fenicia/time-range-selector";
import { CustomerInsights } from "../../../../types/basic/customer/customer-insights";
import { SummaryCards } from "../summary-cards";
import { AtRiskCustomers } from "./at-risk-customers";
import { RecentOrders } from "./recent-orders";
import { TopCustomers } from "./top-customers";

interface CustomerPerformanceInsights {
    insightsLoading: boolean;
    insights: CustomerInsights | null;
    analyticsDays: number;
    setAnalyticsDays: (days: number) => void;
}

export const RenderAnalyticsTab = ({ insightsLoading, insights, analyticsDays, setAnalyticsDays }: CustomerPerformanceInsights) => {
    const { t } = useTranslation();
    if (insightsLoading) {
        return (
            <div className="text-center py-5">
                <CSpinner color="primary" />
                <p className="mt-3">{t("common.loading")}</p>
            </div>
        );
    }

    if (!insights) {
        return (
            <div className="text-center py-5">
                <p className="text-muted">{t("common.noData")}</p>
            </div>
        );
    }

    return (
        <>
            <TimeRangeSelector days={analyticsDays} setDays={setAnalyticsDays} />

            <SummaryCards summary={insights.summary} />

            <TopCustomers topCustomers={insights.topCustomers} />

            <RecentOrders recentOrders={insights.recentOrders} />

            <AtRiskCustomers atRiskCustomers={insights.atRiskCustomers} />
        </>
    );
};
