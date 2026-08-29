import { CSpinner } from "@coreui/react";
import { t } from "i18next";
import TimeRangeSelector from "../../../../components/fenicia/time-range-selector";
import { SupplierPerformance } from "../../../../types/basic/supplier/supplier-performance";
import { CostComparison } from "./cost-comparison";
import { ProductsPerSupplier } from "./products-per-supplier";
import { RecentStockMovement } from "./recent-stock-movement";
import SummaryCards from "./summary-cards";

interface SupplierPerformanceInsights {
    performanceLoading: boolean;
    performance: SupplierPerformance; // Replace with actual type when available
    analyticsDays: number;
    setAnalyticsDays: (days: number) => void;
}

export default function RenderAnalyticsTab({ performanceLoading, performance, analyticsDays, setAnalyticsDays }: SupplierPerformanceInsights) {
    if (performanceLoading) {
        return (
            <div className="text-center py-5">
                <CSpinner color="primary" />
                <p className="mt-3">{t("common.loading")}</p>
            </div>
        );
    }

    if (!performance) {
        return (
            <div className="text-center py-5">
                <p className="text-muted">{t("common.noData")}</p>
            </div>
        );
    }

    return (
        <>
            <TimeRangeSelector days={analyticsDays} setDays={setAnalyticsDays} />

            <SummaryCards data={performance.summary} />

            <ProductsPerSupplier data={performance.productsPerSupplier} />

            <CostComparison data={performance.costComparison} />

            <RecentStockMovement data={performance.recentStockMovements} />
        </>
    );
}
