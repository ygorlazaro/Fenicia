import {
    cilArrowBottom,
    cilArrowTop
} from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CCol,
    CRow,
    CWidgetStatsB
} from '@coreui/react';
import { useTranslation } from 'react-i18next';
import { formatCurrency } from '../../utils/format-currency';
import { formatPercentage } from '../../utils/format-percentage';

interface DailySalesSummaryProps {
    todayRevenue: number;
    todayOrders: number;
    weekRevenue: number;
    weekOrders: number;
    monthRevenue: number;
    growthPercentage: number;
}

const DailySalesSummary = ({ todayRevenue, todayOrders, weekRevenue, weekOrders, monthRevenue, growthPercentage }: DailySalesSummaryProps) => {
    const { t } = useTranslation();

    return (
        <CRow className="mb-4" xs={{ gutter: 4 }}>
            <CCol sm={4} xl={4}>
                <CWidgetStatsB
                    color="primary"
                    title={t('dashboard.today')}
                    value={
                        <>
                            {formatCurrency(todayRevenue)}
                            <span className="fs-6 fw-normal d-block mt-1">
                                {todayOrders} {t('dashboard.orders')}
                            </span>
                        </>
                    }
                />
            </CCol>

            <CCol sm={4} xl={4}>
                <CWidgetStatsB
                    color="info"
                    title={t('dashboard.thisWeek')}
                    value={
                        <>
                            {formatCurrency(weekRevenue)}
                            <span className="fs-6 fw-normal d-block mt-1">
                                {weekOrders} {t('dashboard.orders')}
                            </span>
                        </>
                    }
                />
            </CCol>

            <CCol sm={4} xl={4}>
                <CWidgetStatsB
                    color="success"
                    title={t('dashboard.thisMonth')}
                    value={
                        <>
                            {formatCurrency(monthRevenue)}
                            <span className="fs-6 fw-normal d-block mt-1">
                                <CIcon
                                    icon={growthPercentage >= 0 ? cilArrowTop : cilArrowBottom}
                                    className="me-1"
                                />
                                {formatPercentage(growthPercentage)} {t('dashboard.vsLastMonth')}
                            </span>
                        </>
                    }
                />
            </CCol>
        </CRow>
    );
}

export default DailySalesSummary;
