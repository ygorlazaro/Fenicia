import { cilArrowBottom, cilArrowTop } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import { cilGraph } from '@coreui/icons/dist/esm/free/cil-graph';
import {
    CBadge,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CRow,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow
} from '@coreui/react';
import { CChartBar } from '@coreui/react-chartjs';
import { getStyle } from '@coreui/utils';
import { useTranslation } from 'react-i18next';
import { FinancialProfitMarginTrend } from '../../types/basic/dashboard/financial-profit-margin-trend';
import formatPercentage from '../../utils/format-percentage';

interface ProfitMarginTrendProps {
    profitMarginTrend: FinancialProfitMarginTrend[] 
}
export function ProfitMarginTrend({
    profitMarginTrend
}: ProfitMarginTrendProps) {
    const { t } = useTranslation();

    const getProfitMarginChartData = () => {
        if (profitMarginTrend.length === 0) return null;

        return {
            labels: profitMarginTrend.map(d => d.period),
            datasets: [
                {
                    label: t('dashboard.profitMargin'),
                    backgroundColor: getStyle('--cui-primary'),
                    borderColor: getStyle('--cui-primary'),
                    pointBackgroundColor: getStyle('--cui-primary'),
                    data: profitMarginTrend.map(d => d.marginPercentage),
                },
            ],
        };
    };

    const getTrendIcon = (trend: string) => {
        if (trend === 'Improving') return cilArrowTop;
        if (trend === 'Declining') return cilArrowBottom;
        return null;
    };

    const getTrendColor = (trend: string) => {
        if (trend === 'Improving') return 'success';
        if (trend === 'Declining') return 'danger';
        return 'secondary';
    };

    return <CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilGraph} className="me-2" />
                    <strong>{t('dashboard.profitMarginTrend')}</strong>
                </CCardHeader>
                <CCardBody>
                    {profitMarginTrend.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <>
                        <CChartBar data={getProfitMarginChartData()} options={{
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
                                    beginAtZero: true,
                                    max: 100
                                }
                            }
                        }} />
                        <CTable hover responsive className="mt-3">
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>{t('dashboard.period')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('dashboard.margin')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('dashboard.trend')}</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {profitMarginTrend.slice(-7).map((item, index) => <CTableRow key={index}>
                                    <CTableDataCell>{item.period}</CTableDataCell>
                                    <CTableDataCell className="text-end">
                                        <strong>{formatPercentage(item.marginPercentage)}</strong>
                                    </CTableDataCell>
                                    <CTableDataCell className="text-center">
                                        {getTrendIcon(item.trend) && <CIcon icon={getTrendIcon(item.trend)} className={`text-${getTrendColor(item.trend)}`} size="lg" />}
                                        <CBadge color={getTrendColor(item.trend)} className="ms-2">
                                            {t(`dashboard.${item.trend.toLowerCase()}`)}
                                        </CBadge>
                                    </CTableDataCell>
                                </CTableRow>)}
                            </CTableBody>
                        </CTable>
                    </>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
