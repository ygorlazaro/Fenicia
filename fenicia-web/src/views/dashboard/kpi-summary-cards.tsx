import { cilCart, cilDollar, cilGraph } from '@coreui/icons';
import {
    CRow
} from '@coreui/react';
import { t } from 'i18next';
import formatCurrency from '../../utils/format-currency';
import formatPercentage from '../../utils/format-percentage';
import KpiSummaryCard from './kpi-summary-card';

interface KpiSummaryCardsProps {
    totalRevenue: number;
    totalCost: number;
    grossProfit: number;
    profitMargin: number;
    totalOrders: number;
    averageOrderValue: number;
}

const KpiSummaryCards = ({totalRevenue, totalCost, grossProfit, profitMargin, totalOrders, averageOrderValue}: KpiSummaryCardsProps) => {
    return (
    
        <CRow className="mb-4" xs={{ gutter: 4 }}>
            <KpiSummaryCard value={formatCurrency(totalRevenue)} label={t('dashboard.totalRevenue')} detail={t('dashboard.revenue')} color="success" icon={cilDollar} />
            <KpiSummaryCard value={formatCurrency(totalCost)} label={t('dashboard.totalCost')} detail={t('dashboard.cost')} color="danger" icon={cilCart} />
            <KpiSummaryCard value={formatCurrency(grossProfit)} label={`${formatPercentage(profitMargin)} ${t('dashboard.margin')}`} detail={t('dashboard.grossProfit')} color="info" icon={cilGraph} />
            <KpiSummaryCard value={totalOrders} label={`${formatCurrency(averageOrderValue)} ${t('dashboard.aov')}`} detail={t('dashboard.orders')} color="primary" icon={cilCart} />
        </CRow>
    );
}

export default KpiSummaryCards;
