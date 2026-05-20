import { cilTags } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { CChartBar } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { t } from "i18next";
import { CategoryBreakdown } from "../../../types/basic/inventory/category-breakdown";

interface CategoryChartProps {
    data: CategoryBreakdown[];
}

export default function CategoryChart({ data }: CategoryChartProps) {
    const getCategoryChartData = () => {
        if (!data || data.length === 0) return null;

        return {
            labels: data.map((c) => c.categoryName),
            datasets: [
                {
                    label: t("inventory.costValue"),
                    backgroundColor: getStyle("--cui-danger"),
                    data: data.map((c) => c.totalCostValue)
                },
                {
                    label: t("inventory.salesValue"),
                    backgroundColor: getStyle("--cui-success"),
                    data: data.map((c) => c.totalSalesValue)
                }
            ]
        };
    };

    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader>
                        <CIcon icon={cilTags} className="me-2" />
                        <strong>{t("inventory.categoryComparison")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        <CChartBar
                            data={getCategoryChartData()}
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
                                        ticks: {
                                            callback: (value) => `R$ ${Number(value) / 1000}k`
                                        }
                                    }
                                }
                            }}
                        />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}
