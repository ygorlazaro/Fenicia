import CIcon from "@coreui/icons-react";
import { cilTruck } from "@coreui/icons/dist/esm/free/cil-truck";
import { CCard, CCardBody, CCardHeader, CCol } from "@coreui/react";
import { CChartLine } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { useTranslation } from "react-i18next";
import { RevenueVsCost } from "../../types/basic/dashboard/revenue-vs-cost";

interface RevenueVsCostProps {
    revenueVsCost?: RevenueVsCost[];
}

export default function RevenuwVsCost({ revenueVsCost = [] }: RevenueVsCostProps) {
    const { t } = useTranslation();

    const getRevenueVsCostChartData = () => {
        if (revenueVsCost.length === 0) return null;

        return {
            labels: revenueVsCost.map((d) => new Date(d.date).toLocaleDateString()),
            datasets: [
                {
                    label: t("dashboard.revenue"),
                    backgroundColor: "transparent",
                    borderColor: getStyle("--cui-success"),
                    pointBackgroundColor: getStyle("--cui-success"),
                    data: revenueVsCost.map((d) => d.revenue)
                },
                {
                    label: t("dashboard.cost"),
                    backgroundColor: "transparent",
                    borderColor: getStyle("--cui-danger"),
                    pointBackgroundColor: getStyle("--cui-danger"),
                    data: revenueVsCost.map((d) => d.cost)
                },
                {
                    label: t("dashboard.profit"),
                    backgroundColor: "transparent",
                    borderColor: getStyle("--cui-info"),
                    pointBackgroundColor: getStyle("--cui-info"),
                    data: revenueVsCost.map((d) => d.profit)
                }
            ]
        };
    };

    return (
        <CCol md={8}>
            <CCard className="mb-4">
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilTruck} className="me-2" />
                    <strong>{t("dashboard.revenueVsCost")}</strong>
                </CCardHeader>
                <CCardBody>
                    {revenueVsCost?.length === 0 ? (
                        <p className="text-muted text-center">{t("common.noData")}</p>
                    ) : (
                        <CChartLine
                            data={getRevenueVsCostChartData()}
                            options={{
                                responsive: true,
                                maintainAspectRatio: true,
                                plugins: {
                                    legend: {
                                        position: "top"
                                    }
                                },
                                scales: {
                                    x: {
                                        grid: {
                                            display: false
                                        }
                                    },
                                    y: {
                                        beginAtZero: true
                                    }
                                }
                            }}
                        />
                    )}
                </CCardBody>
            </CCard>
        </CCol>
    );
}
