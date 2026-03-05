import {
    cilArrowBottom,
    cilArrowTop,
    cilCalendar,
    cilCart,
    cilCheck,
    cilClock,
    cilDollar,
    cilGraph,
    cilTruck,
    cilWarning
} from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CAlert,
    CBadge,
    CButton,
    CButtonGroup,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CProgress,
    CRow,
    CSpinner,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CWidgetStatsA,
    CWidgetStatsB
} from '@coreui/react';
import { CChartBar, CChartDoughnut, CChartLine } from '@coreui/react-chartjs';
import { getStyle } from '@coreui/utils';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import FinancialDashboardClient from '../../services/financial-dashboard-client';

const dashboardClient = new FinancialDashboardClient();

const Dashboard = () => {
    const { t } = useTranslation();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [dashboard, setDashboard] = useState(null);
    const [days, setDays] = useState(90);

    useEffect(() => {
        loadDashboard();
    }, [days]);

    const loadDashboard = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await dashboardClient.getFinancialDashboard(days);
            setDashboard(data);
        } catch (err) {
            setError(t('dashboard.loadError'));
            console.error('Failed to load financial dashboard:', err);
        } finally {
            setLoading(false);
        }
    };

    const formatCurrency = (value: number) => {
        if (value === null || value === undefined) return '-';
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
        }).format(value);
    };

    const formatNumber = (value: number) => {
        return new Intl.NumberFormat('pt-BR').format(value);
    };

    const formatPercentage = (value: number) => {
        return `${value.toFixed(1)}%`;
    };

    const getRevenueVsCostChartData = () => {
        if (!dashboard || dashboard.revenueVsCost.length === 0) return null;

        return {
            labels: dashboard.revenueVsCost.map(d => new Date(d.date).toLocaleDateString()),
            datasets: [
                {
                    label: t('dashboard.revenue'),
                    backgroundColor: 'transparent',
                    borderColor: getStyle('--cui-success'),
                    pointBackgroundColor: getStyle('--cui-success'),
                    data: dashboard.revenueVsCost.map(d => d.revenue),
                },
                {
                    label: t('dashboard.cost'),
                    backgroundColor: 'transparent',
                    borderColor: getStyle('--cui-danger'),
                    pointBackgroundColor: getStyle('--cui-danger'),
                    data: dashboard.revenueVsCost.map(d => d.cost),
                },
                {
                    label: t('dashboard.profit'),
                    backgroundColor: 'transparent',
                    borderColor: getStyle('--cui-info'),
                    pointBackgroundColor: getStyle('--cui-info'),
                    data: dashboard.revenueVsCost.map(d => d.profit),
                },
            ],
        };
    };

    const getProfitMarginChartData = () => {
        if (!dashboard || dashboard.profitMarginTrend.length === 0) return null;

        return {
            labels: dashboard.profitMarginTrend.map(d => d.period),
            datasets: [
                {
                    label: t('dashboard.profitMargin'),
                    backgroundColor: getStyle('--cui-primary'),
                    borderColor: getStyle('--cui-primary'),
                    pointBackgroundColor: getStyle('--cui-primary'),
                    data: dashboard.profitMarginTrend.map(d => d.marginPercentage),
                },
            ],
        };
    };

    const getAccountsReceivableChartData = () => {
        if (!dashboard) return null;

        return {
            labels: [t('dashboard.pending'), t('dashboard.approved')],
            datasets: [
                {
                    backgroundColor: [getStyle('--cui-warning'), getStyle('--cui-success')],
                    data: [dashboard.accountsReceivable.totalPending, dashboard.accountsReceivable.totalApproved],
                },
            ],
        };
    };

    const getGrowthBadgeColor = (growth: number) => {
        if (growth >= 10) return 'success';
        if (growth >= 0) return 'info';
        return 'danger';
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

    if (loading) {
        return (
            <CContainer className="py-4">
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            </CContainer>
        );
    }

    if (error || !dashboard) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error || t('common.noData')}
                </CAlert>
            </CContainer>
        );
    }

    return (
        <CContainer className="py-4">
            {/* Time Range Selector */}
            <CRow className="mb-4">
                <CCol xs={12}>
                    <div className="d-flex justify-content-between align-items-center">
                        <h4 className="mb-0">{t('dashboard.financialDashboard')}</h4>
                        <CButtonGroup>
                            <CButton
                                color={days === 30 ? 'primary' : 'outline-primary'}
                                onClick={() => setDays(30)}
                            >
                                {t('dashboard.last30Days')}
                            </CButton>
                            <CButton
                                color={days === 90 ? 'primary' : 'outline-primary'}
                                onClick={() => setDays(90)}
                            >
                                {t('dashboard.last90Days')}
                            </CButton>
                            <CButton
                                color={days === 180 ? 'primary' : 'outline-primary'}
                                onClick={() => setDays(180)}
                            >
                                {t('dashboard.last180Days')}
                            </CButton>
                        </CButtonGroup>
                    </div>
                </CCol>
          </CRow>

            {/* KPI Summary Cards */}
            <CRow className="mb-4" xs={{ gutter: 4 }}>
                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="success"
                        value={
                            <>
                                {formatCurrency(dashboard.kpi.totalRevenue)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {t('dashboard.totalRevenue')}
                                </span>
                            </>
                        }
                        title={t('dashboard.revenue')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilDollar} size="xl" className="text-white-50" />
                            </div>
            }
                    />
              </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="danger"
                        value={
                            <>
                                {formatCurrency(dashboard.kpi.totalCost)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {t('dashboard.totalCost')}
                                </span>
                            </>
                        }
                        title={t('dashboard.cost')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilCart} size="xl" className="text-white-50" />
                            </div>
            }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="info"
                        value={
                            <>
                                {formatCurrency(dashboard.kpi.grossProfit)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {formatPercentage(dashboard.kpi.profitMargin)} {t('dashboard.margin')}
                                </span>
                            </>
                        }
                        title={t('dashboard.grossProfit')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilGraph} size="xl" className="text-white-50" />
                            </div>
            }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="primary"
                        value={
                            <>
                                {dashboard.kpi.totalOrders}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {formatCurrency(dashboard.kpi.averageOrderValue)} {t('dashboard.aov')}
                                </span>
                            </>
                        }
                        title={t('dashboard.orders')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilCart} size="xl" className="text-white-50" />
                            </div>
            }
                    />
                </CCol>
            </CRow>

            {/* Daily Sales Summary */}
            <CRow className="mb-4" xs={{ gutter: 4 }}>
                <CCol sm={4} xl={4}>
                    <CWidgetStatsB
                        color="primary"
                        title={t('dashboard.today')}
                        value={
                            <>
                                {formatCurrency(dashboard.dailySales.todayRevenue)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {dashboard.dailySales.todayOrders} {t('dashboard.orders')}
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
                                {formatCurrency(dashboard.dailySales.weekRevenue)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {dashboard.dailySales.weekOrders} {t('dashboard.orders')}
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
                                {formatCurrency(dashboard.dailySales.monthRevenue)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    <CIcon
                                        icon={dashboard.dailySales.growthPercentage >= 0 ? cilArrowTop : cilArrowBottom}
                                        className="me-1"
                                    />
                                    {formatPercentage(dashboard.dailySales.growthPercentage)} {t('dashboard.vsLastMonth')}
                                </span>
                            </>
                        }
                    />
                </CCol>
            </CRow>

            {/* Charts Row */}
            <CRow className="mb-4" xs={{ gutter: 4 }}>
                <CCol md={8}>
                    <CCard className="mb-4">
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilTruck} className="me-2" />
                            <strong>{t('dashboard.revenueVsCost')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {dashboard.revenueVsCost.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <CChartLine
                                    data={getRevenueVsCostChartData()}
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
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol md={4}>
                    <CCard className="mb-4">
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilClock} className="me-2" />
                            <strong>{t('dashboard.accountsReceivable')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {dashboard.accountsReceivable.totalPending === 0 && dashboard.accountsReceivable.totalApproved === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <>
                                    <CChartDoughnut
                                        data={getAccountsReceivableChartData()}
                                        options={{
                                            responsive: true,
                                            maintainAspectRatio: true,
                                            plugins: {
                                                legend: {
                                                    position: 'bottom',
                                                },
                                            },
                                        }}
                                    />
                                    <CRow className="mt-3" xs={{ gutter: 2 }}>
                                        <CCol xs={6}>
                                            <div className="text-center">
                                                <div className="text-warning fw-semibold">
                                                    {formatCurrency(dashboard.accountsReceivable.totalPending)}
                                                </div>
                                                    <small className="text-body-secondary">
                                                        {dashboard.accountsReceivable.pendingOrdersCount} {t('dashboard.pendingOrders')}
                                                    </small>
                      </div>
                                            </CCol>
                                            <CCol xs={6}>
                                                <div className="text-center">
                                                    <div className="text-success fw-semibold">
                                                        {formatCurrency(dashboard.accountsReceivable.totalApproved)}
                                                    </div>
                                                    <small className="text-body-secondary">
                                                        {dashboard.accountsReceivable.approvedOrdersCount} {t('dashboard.approvedOrders')}
                                                    </small>
                                                </div>
                                        </CCol>
                                    </CRow>
                                </>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            {/* Profit Margin Trend */}
            <CRow className="mb-4">
                <CCol xs={12}>
                    <CCard>
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilGraph} className="me-2" />
                            <strong>{t('dashboard.profitMarginTrend')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {dashboard.profitMarginTrend.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <>
                                    <CChartBar
                                        data={getProfitMarginChartData()}
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
                                                    max: 100,
                                                },
                                            },
                                        }}
                                    />
                                    <CTable hover responsive className="mt-3">
                                        <CTableHead>
                                            <CTableRow>
                                                    <CTableHeaderCell>{t('dashboard.period')}</CTableHeaderCell>
                                                    <CTableHeaderCell className="text-end">{t('dashboard.margin')}</CTableHeaderCell>
                                                    <CTableHeaderCell className="text-center">{t('dashboard.trend')}</CTableHeaderCell>
                                                </CTableRow>
                                            </CTableHead>
                                            <CTableBody>
                                                {dashboard.profitMarginTrend.slice(-7).map((item, index) => (
                                                    <CTableRow key={index}>
                                                        <CTableDataCell>{item.period}</CTableDataCell>
                                                        <CTableDataCell className="text-end">
                                                            <strong>{formatPercentage(item.marginPercentage)}</strong>
                                                        </CTableDataCell>
                                                        <CTableDataCell className="text-center">
                                                            {getTrendIcon(item.trend) && (
                                                                <CIcon
                                                                    icon={getTrendIcon(item.trend)}
                                                                    className={`text-${getTrendColor(item.trend)}`}
                                                                    size="lg"
                                                                />
                                                            )}
                                                            <CBadge color={getTrendColor(item.trend)} className="ms-2">
                                                                {t(`dashboard.${item.trend.toLowerCase()}`)}
                                                            </CBadge>
                                                        </CTableDataCell>
                                                    </CTableRow>
                                                ))}
                                            </CTableBody>
                                        </CTable>
                                </>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            {/* Additional Info */}
            <CRow xs={{ gutter: 4 }}>
                <CCol md={6}>
                    <CCard>
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilCalendar} className="me-2" />
                            <strong>{t('dashboard.stockValue')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            <div className="text-center py-3">
                                <div className="fs-2 fw-semibold text-primary">
                                    {formatCurrency(dashboard.kpi.totalStockValue)}
                                </div>
                                <p className="text-muted mb-0">{t('dashboard.totalStockValue')}</p>
                                <div className="mt-3">
                                    <CProgress
                                        value={100}
                                        color="primary"
                                        className="mb-2"
                                    />
                                    <small className="text-body-secondary">
                                        {dashboard.kpi.totalProducts} {t('dashboard.products')}
                                    </small>
                                </div>
                            </div>
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol md={6}>
                    <CCard>
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilWarning} className="me-2" />
                            <strong>{t('dashboard.pendingOrdersAlert')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {dashboard.accountsReceivable.pendingOrdersCount > 0 ? (
                                <div className="text-center py-3">
                                    <div className="fs-2 fw-semibold text-warning">
                                        {dashboard.accountsReceivable.pendingOrdersCount}
                                    </div>
                                    <p className="text-muted mb-0">{t('dashboard.pendingOrdersAwaitingApproval')}</p>
                                    <div className="mt-3">
                                        <CProgress
                                            value={(dashboard.accountsReceivable.pendingOrdersCount / (dashboard.accountsReceivable.pendingOrdersCount + dashboard.accountsReceivable.approvedOrdersCount)) * 100}
                                            color="warning"
                                            className="mb-2"
                                        />
                                        <small className="text-body-secondary">
                                            {formatCurrency(dashboard.accountsReceivable.totalPending)} {t('dashboard.pendingValue')}
                                        </small>
                                    </div>
                                </div>
                            ) : (
                                <div className="text-center py-3">
                                    <CIcon icon={cilCheck} className="text-success" size="4xl" />
                                    <p className="text-muted mt-2 mb-0">{t('dashboard.noPendingOrders')}</p>
                                </div>
                            )}
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
        </CContainer>
    );
};

export default Dashboard;
