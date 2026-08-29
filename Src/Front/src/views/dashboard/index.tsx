// @ts-nocheck
import { CAlert, CContainer, CSpinner } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import TimeRangeSelector from "../../components/fenicia/time-range-selector";
import { BasicDataSourceClient } from "../../services/basic/basic-datasource-client";
import { FinancialDashboard } from "../../types/basic/dashboard/financial-dashboard";
import { AdditionalInfo } from "./additional-info";
import ChartsRow from "./charts-row";
import DailySalesSummary from "./daily-sales-summary";
import KpiSummaryCards from "./kpi-summary-cards";
import { ProfitMarginTrend } from "./profit-margin-trend";

const datasourceClient = new BasicDataSourceClient();

const Dashboard = () => {
    const { t } = useTranslation();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [dashboard, setDashboard] = useState<FinancialDashboard | null>(null);
    const [days, setDays] = useState(90);

    useEffect(() => {
        loadDashboard();
    }, [days]);

    const loadDashboard = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await datasourceClient.getFinancialDashboard(days);
            setDashboard(data);
        } catch (err) {
            setError(t("dashboard.loadError"));
            console.error("Failed to load financial dashboard:", err);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <CContainer className="py-4">
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t("common.loading")}</p>
                </div>
            </CContainer>
        );
    }

    if (error || !dashboard) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error || t("common.noData")}
                </CAlert>
            </CContainer>
        );
    }

    return (
        <CContainer className="py-4">
            {/* Time Range Selector */}
            <TimeRangeSelector days={days} setDays={setDays} title={t("dashboard.financialDashboard")} />

            {/* KPI Summary Cards */}
            <KpiSummaryCards totalRevenue={dashboard.kpi.totalRevenue} totalCost={dashboard.kpi.totalCost} grossProfit={dashboard.kpi.grossProfit} profitMargin={dashboard.kpi.profitMargin} totalOrders={dashboard.kpi.totalOrders} averageOrderValue={dashboard.kpi.averageOrderValue} />

            {/* Daily Sales Summary */}
            <DailySalesSummary todayRevenue={dashboard.dailySales.todayRevenue} todayOrders={dashboard.dailySales.todayOrders} weekRevenue={dashboard.dailySales.weekRevenue} weekOrders={dashboard.dailySales.weekOrders} monthRevenue={dashboard.dailySales.monthRevenue} growthPercentage={dashboard.dailySales.growthPercentage} />

            {/* Charts Row */}
            <ChartsRow revenueVsCost={dashboard.revenueVsCost} accountsReceivable={dashboard.accountsReceivable} />

            {/* Profit Margin Trend */}
            <ProfitMarginTrend profitMarginTrend={dashboard.profitMarginTrend} />

            {/* Additional Info */}
            <AdditionalInfo kpi={dashboard.kpi} accountsReceivable={dashboard.accountsReceivable} />
        </CContainer>
    );
};

export default Dashboard;
