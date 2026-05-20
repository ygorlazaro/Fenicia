import { CCol, CRow, CWidgetStatsA } from "@coreui/react";
import { t } from "i18next";
import { EmployeePerformanceSummary } from "../../../../types/basic/employee/employee-performance-summary";
import formatCurrency from "../../../../utils/format-currency";

interface SummaryCardsProps {
    summary: EmployeePerformanceSummary;
}
export function SummaryCards({ summary }: SummaryCardsProps) {
    return (
        <CRow
            className="mb-4"
            xs={{
                gutter: 4
            }}
        >
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="primary"
                    value={
                        <>
                            {summary.activeEmployees}
                            <span className="fs-6 fw-normal d-block mt-1">
                                / {summary.totalEmployees} {t("employees.employees")}
                            </span>
                        </>
                    }
                    title={t("employees.activeEmployees")}
                />
            </CCol>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="success"
                    value={
                        <>
                            {formatCurrency(summary.totalSales)}
                            <span className="fs-6 fw-normal d-block mt-1">{t("employees.totalSales")}</span>
                        </>
                    }
                    title={t("employees.totalSales")}
                />
            </CCol>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="info"
                    value={
                        <>
                            {summary.totalOrders}
                            <span className="fs-6 fw-normal d-block mt-1">{t("employees.totalOrders")}</span>
                        </>
                    }
                    title={t("employees.totalOrders")}
                />
            </CCol>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="warning"
                    value={
                        <>
                            {formatCurrency(summary.averageSalesPerEmployee)}
                            <span className="fs-6 fw-normal d-block mt-1">{t("employees.avgPerEmployee")}</span>
                        </>
                    }
                    title={t("employees.averageSalesPerEmployee")}
                />
            </CCol>
        </CRow>
    );
}
