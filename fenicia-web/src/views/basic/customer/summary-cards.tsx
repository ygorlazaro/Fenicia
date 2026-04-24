import { CCol, CRow, CWidgetStatsA } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { CustomerSummary } from '../../../types/basic/customer/customer-summary';
import formatCurrency from "../../../utils/format-currency";

interface SummaryCardsProps {
    summary: CustomerSummary;
}

export function SummaryCards({ summary }: SummaryCardsProps) {
    const { t } = useTranslation();
    
    return <CRow className="mb-4" xs={{
        gutter: 4
    }}>
        <CCol sm={6} xl={3}>
            <CWidgetStatsA color="primary" value={<>
                {summary.totalCustomers}
                <span className="fs-6 fw-normal d-block mt-1">
                    {t('customers.customers')}
                </span>
            </>} title={t('customers.totalCustomers')} />
        </CCol>
        <CCol sm={6} xl={3}>
            <CWidgetStatsA color="success" value={<>
                {summary.totalOrders}
                <span className="fs-6 fw-normal d-block mt-1">
                    {t('customers.orders')}
                </span>
            </>} title={t('customers.totalOrders')} />
        </CCol>
        <CCol sm={6} xl={3}>
            <CWidgetStatsA color="info" value={<>
                {formatCurrency(summary.totalRevenue)}
                <span className="fs-6 fw-normal d-block mt-1">
                    {t('customers.revenue')}
                </span>
            </>} title={t('customers.totalRevenue')} />
        </CCol>
        <CCol sm={6} xl={3}>
            <CWidgetStatsA color="warning" value={<>
                {formatCurrency(summary.averageOrderValue)}
                <span className="fs-6 fw-normal d-block mt-1">
                    {t('customers.aov')}
                </span>
            </>} title={t('customers.averageOrderValue')} />
        </CCol>
    </CRow>;
}
