import { cilLayers } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { CChartBar } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { useTranslation } from "react-i18next";
import { MonthlyInOut } from "../../../types/basic/stock-movement/monthly-in-out";

interface MonthlyIdOutChartProps {
    monthlyInOut: MonthlyInOut[] | null;
}

export default function MonthlyIdOutChart({ monthlyInOut = [] }: MonthlyIdOutChartProps) {
    const { t } = useTranslation();

    const getMonthlyInOutChartData = () => {
        return {
            labels: monthlyInOut?.map(m => m.month) || [],
            datasets: [
                {
                    label: t('stockMovement.in'),
                    backgroundColor: getStyle('--cui-success'),
                    data: monthlyInOut?.map(m => m.totalIn) || [],
                },
                {
                    label: t('stockMovement.out'),
                    backgroundColor: getStyle('--cui-danger'),
                    data: monthlyInOut?.map(m => m.totalOut) || [],
                },
            ],
        };
    };

    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilLayers} className="me-2" size="lg" />
                        <strong>{t('stockMovement.monthlyInOut')}</strong>
                    </CCardHeader>
                    <CCardBody>
                        <CChartBar
                            data={getMonthlyInOutChartData()}
                            options={{
                                responsive: true,
                                maintainAspectRatio: true,
                                plugins: {
                                    legend: {
                                        position: 'top',
                                    },
                                },
                                scales: {
                                    x: {
                                        grid: {
                                            display: false,
                                        },
                                    },
                                    y: {
                                        beginAtZero: true,
                                    },
                                },
                            }}
                        />
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    )
}
