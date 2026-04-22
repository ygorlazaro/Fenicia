import { CSpinner } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import TimeRangeSelector from "../../../../components/fenicia/time-range-selector";
import EmployeePerformanceClient, { EmployeePerformance } from "../../../../services/employee-performance-client";
import { OrdersByEmployee } from './orders-by-employee';
import { SalesByEmployee } from './sales-by-employee';
import { SummaryCards } from './summary-cards';
import { TopPerformers } from './top-performers';

interface EmployeePerformanceData {
    analyticsDays: number;
    activeTab: number;
    setAnalyticsDays: (days: number) => void;
    onError?: (message: string) => void;
}

const performanceClient = new EmployeePerformanceClient();

export function RenderPerformanceTab({ analyticsDays, setAnalyticsDays, activeTab, onError }: EmployeePerformanceData) {
    const { t } = useTranslation();
    const [performanceLoading, setPerformanceLoading] = useState(false);
    const [performance, setPerformance] = useState<EmployeePerformance | null>(null);

    useEffect(() => {
        if (activeTab === 1) {
            loadPerformance();
        }
    }, [activeTab, analyticsDays]);

    const loadPerformance = async () => {
        try {
            setPerformanceLoading(true);
            const data = await performanceClient.getPerformance(analyticsDays);
            setPerformance(data);
        } catch (err) {
            onError?.(t('employees.performanceLoadError'));
            console.error('Failed to load employee performance:', err);
        } finally {
            setPerformanceLoading(false);
        }
    };


    if (performanceLoading) {
        return <div className="text-center py-5">
            <CSpinner color="primary" />
            <p className="mt-3">{t('common.loading')}</p>
        </div>;
    }

    if (!performance) {
        return <div className="text-center py-5">
            <p className="text-muted">{t('common.noData')}</p>
        </div>;
    }

    return <>
        <TimeRangeSelector days={analyticsDays} setDays={setAnalyticsDays} />

        <SummaryCards summary={performance.summary} />

        <SalesByEmployee data={performance.salesByEmployee} />

        <TopPerformers topPerformers={performance.topPerformers} />

        <OrdersByEmployee ordersByEmployee={performance.ordersByEmployee} />
    </>;
}
