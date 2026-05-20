import { cilChart } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { CChartBar } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { useTranslation } from "react-i18next";
import { EmployeeSales } from "../../../../types/basic/employee/employee-sales";

interface SalesByEmployeeProps {
    data: EmployeeSales[];
}

export function SalesByEmployee({ data }: SalesByEmployeeProps) {
    const { t } = useTranslation();

    const getSalesChartData = () => {
        if (data.length === 0) return null;

        return {
            labels: data.slice(0, 10).map((e) => e.employeeName.split(" ")[0]),
            datasets: [
                {
                    label: t("employees.totalSales"),
                    backgroundColor: getStyle("--cui-primary"),
                    data: data.slice(0, 10).map((e) => e.totalSales)
                }
            ]
        };
    };

    return (
        <CRow className="mb-4">
            <CCol md={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilChart} className="me-2" />
                        <strong>{t("employees.salesByEmployee")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {data.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CChartBar
                                data={getSalesChartData()}
                                options={{
                                    responsive: true,
                                    maintainAspectRatio: true,
                                    plugins: {
                                        legend: {
                                            display: false
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
        </CRow>
    );
}
