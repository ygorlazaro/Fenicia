import { CSpinner } from "@coreui/react";
import { t } from "i18next";
import TimeRangeSelector from "../../../../components/fenicia/time-range-selector";
import { OrderAnalytics } from "../../../../types/basic/order/order-analytics";
import CancelledOrder from "./cancelled-order";
import ChartsRow from "./charts-row";
import SummaryCards from "./summary-cards";
import TopCustomers from "./top-customers";

interface RenderAnalyticsTabProps {
    analytics: OrderAnalytics | null;
    analyticsLoading: boolean;
    analyticsDays: number;
    setAnalyticsDays: (days: number) => void;
}

export default function RenderAnalyticsTab({ analytics, analyticsLoading, analyticsDays, setAnalyticsDays }: RenderAnalyticsTabProps) {
    if (analyticsLoading) {
        return (
            <div className="text-center py-5">
                <CSpinner color="primary" />
                <p className="mt-3">{t('common.loading')}</p>
            </div>
        );
    }

    if (!analytics) {
        return (
            <div className="text-center py-5">
                <p className="text-muted">{t('common.noData')}</p>
            </div>
        );
    }

    return (
        <>
            <TimeRangeSelector days={analyticsDays} setDays={setAnalyticsDays} />

            <SummaryCards averageOrderValue={analytics.averageOrderValue} cancelledOrderLength={analytics.cancelledOrders.length} />

            <ChartsRow analytics={analytics} />

            <TopCustomers topCustomers={analytics.topCustomers} />

            <CancelledOrder cancelledOrders={analytics.cancelledOrders} />
        </>
    );
}
