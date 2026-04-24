import { cilChart } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { CChartLine, CChartPie } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { t } from "i18next";
import { OrderAnalytics } from "../../../../types/basic/order/order-analytics";

interface ChartsRowProps {
    analytics: OrderAnalytics;
}

export default function ChartsRow({ analytics }: ChartsRowProps) {

    const getOrdersByStatusChartData = () => {
        if (!analytics || analytics.ordersByStatus.length === 0) return null;

        return {
            labels: analytics.ordersByStatus.map(s => t(`orders.statusValues.${s.status.toLowerCase()}`)),
            datasets: [
                {
                    label: t('orders.orders'),
                    backgroundColor: [
                        getStyle('--cui-warning'),
                        getStyle('--cui-success'),
                        getStyle('--cui-danger')
                    ],
                    data: analytics.ordersByStatus.map(s => s.count),
                },
            ],
        };
    };

    const getSalesTrendChartData = () => {
        if (!analytics || analytics.salesTrend.length === 0) return null;

        return {
            labels: analytics.salesTrend.map(s => s.period),
            datasets: [
                {
                    label: t('orders.revenue'),
                    backgroundColor: getStyle('--cui-primary'),
                    borderColor: getStyle('--cui-primary'),
                    data: analytics.salesTrend.map(s => s.totalValue),
                    tension: 0.4,
                },
            ],
        };
    };

    return (<CRow className="mb-4" xs={{ gutter: 4 }}>
        <CCol md={6}>
            <CCard className="mb-4">
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilChart} className="me-2" />
                    <strong>{t('orders.ordersByStatus')}</strong>
                </CCardHeader>
                <CCardBody>
                    {analytics.ordersByStatus.length === 0 ? (
                        <p className="text-muted text-center">{t('common.noData')}</p>
                    ) : (
                        <CChartPie
                            data={getOrdersByStatusChartData()}
                            options={{
                                responsive: true,
                                maintainAspectRatio: true,
                                plugins: {
                                    legend: {
                                        position: 'bottom',
                                    },
                                },
                            }} />
                    )}
                </CCardBody>
            </CCard>
        </CCol>

        <CCol md={6}>
            <CCard className="mb-4">
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilChart} className="me-2" />
                    <strong>{t('orders.salesTrend')}</strong>
                </CCardHeader>
                <CCardBody>
                    {analytics.salesTrend.length === 0 ? (
                        <p className="text-muted text-center">{t('common.noData')}</p>
                    ) : (
                        <CChartLine
                            data={getSalesTrendChartData()}
                            options={{
                                responsive: true,
                                maintainAspectRatio: true,
                                plugins: {
                                    legend: {
                                        display: false,
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
                            }} />
                    )}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>
    );
}
