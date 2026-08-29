import { CSpinner } from "@coreui/react";
import { useTranslation } from "react-i18next";
import TimeRangeSelector from "../../../../components/fenicia/time-range-selector";
import { ProductPerformance } from "../../../../types/basic/product/product-performance";
import BestSellingProducts from "./best-selling-products";
import ProductNeverSold from "./products-never-sold";
import ProfitMargins from "./profit-margins";
import WorstSellingProducts from "./worst-selling-products";

interface RenderAnalyticsTabProps {
    performance: ProductPerformance | null;
    performanceLoading: boolean;
    analyticsDays: number;
    setAnalyticsDays: (days: number) => void;
}

export default function RenderAnalyticsTab({ performance, performanceLoading, analyticsDays, setAnalyticsDays }: RenderAnalyticsTabProps) {
    const { t } = useTranslation();

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

            <BestSellingProducts data={performance.bestSellingProducts} />

            <WorstSellingProducts data={performance.worstSellingProducts} />

            <ProfitMargins data={performance.profitMargins} />

            <ProductNeverSold data={performance.neverSoldProducts} />
        </>
    );
}
