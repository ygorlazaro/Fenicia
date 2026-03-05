import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import {
    CCard,
    CCardBody,
    CCardHeader,
    CContainer,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CSpinner,
    CAlert,
    CRow,
    CCol,
    CWidgetStatsA
} from '@coreui/react';
import { CChartBar, CChartLine } from '@coreui/react-chartjs';
import CIcon from '@coreui/icons-react';
import { cilArrowBottom, cilArrowTop, cilLayers, cilSpeedometer, cilHistory } from '@coreui/icons';
import { StockMovementClient, StockMovementDashboard, StockMovementHistory, MonthlyInOut, TopMovedProduct, StockTurnover } from '../../../services/stock-movement-client';
import { BasicProductClient, BasicOrderClient, BasicCustomerClient, BasicEmployeeClient } from '../../../services/basic-crud-clients';
import { getStyle } from '@coreui/utils';
import ProductModal from '../../../components/ProductModal';
import CustomerModal from '../../../components/CustomerModal';
import EmployeeModal from '../../../components/EmployeeModal';

const stockMovementClient = new StockMovementClient();

const StockMovementDashboardView = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [dashboard, setDashboard] = useState<StockMovementDashboard | null>(null);
    const [days, setDays] = useState(30);
    
    // Modal state for quick view without navigation
    const [productModalVisible, setProductModalVisible] = useState(false)
    const [orderModalVisible, setOrderModalVisible] = useState(false)
    const [selectedItem, setSelectedItem] = useState(null)
    const [modalLoading, setModalLoading] = useState(false)
    
    const productClient = new BasicProductClient()
    const orderClient = new BasicOrderClient()

    useEffect(() => {
        loadDashboard();
    }, [days]);

    const loadDashboard = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await stockMovementClient.getDashboard(days);
            setDashboard(data);
        } catch (err) {
            setError(t('stockMovement.loadError'));
            console.error('Failed to load stock movement dashboard:', err);
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

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString();
    };

    // Open modal without navigation
    const openProductModal = async (productId: string, e: React.MouseEvent) => {
        e.preventDefault()
        e.stopPropagation()
        try {
            setModalLoading(true)
            const product = await productClient.getById(productId)
            setSelectedItem(product)
            setProductModalVisible(true)
        } catch (err) {
            console.error('Failed to load product:', err)
            navigate(`/basic/products?id=${productId}`)
        } finally {
            setModalLoading(false)
        }
    }

    const openOrderModal = async (orderId: string, e: React.MouseEvent) => {
        e.preventDefault()
        e.stopPropagation()
        try {
            setModalLoading(true)
            const order = await orderClient.getById(orderId)
            setSelectedItem(order)
            setOrderModalVisible(true)
        } catch (err) {
            console.error('Failed to load order:', err)
            navigate(`/basic/order/${orderId}`)
        } finally {
            setModalLoading(false)
        }
    }

    const getTypeBadgeColor = (type: string) => {
        return type === 'In' ? 'success' : 'danger';
    };

    const getTurnoverBadgeColor = (classification: string) => {
        switch (classification.toLowerCase()) {
            case 'high':
                return 'success';
            case 'medium':
                return 'warning';
            case 'low':
                return 'orange';
            default:
                return 'danger';
        }
    };

    const getMonthlyInOutChartData = () => {
        if (!dashboard || !dashboard.monthlyInOut || dashboard.monthlyInOut.length === 0) return null;

        return {
            labels: dashboard.monthlyInOut.map(m => m.month),
            datasets: [
                {
                    label: t('stockMovement.in'),
                    backgroundColor: getStyle('--cui-success'),
                    data: dashboard.monthlyInOut.map(m => m.totalIn),
                },
                {
                    label: t('stockMovement.out'),
                    backgroundColor: getStyle('--cui-danger'),
                    data: dashboard.monthlyInOut.map(m => m.totalOut),
                },
            ],
        };
    };

    const getTotalInQuantity = () => {
        if (!dashboard || !dashboard.monthlyInOut) return 0;
        return dashboard.monthlyInOut.reduce((sum, m) => sum + m.totalIn, 0);
    };

    const getTotalOutQuantity = () => {
        if (!dashboard || !dashboard.monthlyInOut) return 0;
        return dashboard.monthlyInOut.reduce((sum, m) => sum + m.totalOut, 0);
    };

    const getTotalMovements = () => {
        if (!dashboard || !dashboard.history) return 0;
        return dashboard.history.length;
    };

    const getAverageTurnover = () => {
        if (!dashboard || !dashboard.turnoverRates || dashboard.turnoverRates.length === 0) return 0;
        const sum = dashboard.turnoverRates.reduce((acc, t) => acc + t.turnoverRate, 0);
        return (sum / dashboard.turnoverRates.length).toFixed(2);
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

    if (error) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
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
                        <h4 className="mb-0">{t('stockMovement.dashboard')}</h4>
                        <div className="d-flex gap-2">
                            <button
                                className={`btn btn-sm ${days === 7 ? 'btn-primary' : 'btn-outline-primary'}`}
                                onClick={() => setDays(7)}
                            >
                                {t('stockMovement.last7Days')}
                            </button>
                            <button
                                className={`btn btn-sm ${days === 30 ? 'btn-primary' : 'btn-outline-primary'}`}
                                onClick={() => setDays(30)}
                            >
                                {t('stockMovement.last30Days')}
                            </button>
                            <button
                                className={`btn btn-sm ${days === 90 ? 'btn-primary' : 'btn-outline-primary'}`}
                                onClick={() => setDays(90)}
                            >
                                {t('stockMovement.last90Days')}
                            </button>
                        </div>
                    </div>
                </CCol>
            </CRow>

            {/* Summary Cards */}
            <CRow className="mb-4" xs={{ gutter: 4 }}>
                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="success"
                        value={
                            <>
                                {formatNumber(getTotalInQuantity())}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {t('stockMovement.unitsIn')}
                                </span>
                            </>
                        }
                        title={t('stockMovement.totalIn')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilArrowTop} size="xl" className="text-white-50" />
                            </div>
                        }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="danger"
                        value={
                            <>
                                {formatNumber(getTotalOutQuantity())}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {t('stockMovement.unitsOut')}
                                </span>
                            </>
                        }
                        title={t('stockMovement.totalOut')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilArrowBottom} size="xl" className="text-white-50" />
                            </div>
                        }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="primary"
                        value={
                            <>
                                {getTotalMovements()}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {t('stockMovement.movements')}
                                </span>
                            </>
                        }
                        title={t('stockMovement.totalMovements')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilHistory} size="xl" className="text-white-50" />
                            </div>
                        }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="info"
                        value={
                            <>
                                {getAverageTurnover()}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {t('stockMovement.avgTurnover')}
                                </span>
                            </>
                        }
                        title={t('stockMovement.averageTurnover')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilSpeedometer} size="xl" className="text-white-50" />
                            </div>
                        }
                    />
                </CCol>
            </CRow>

            {/* Monthly In vs Out Chart */}
            {dashboard && dashboard.monthlyInOut && dashboard.monthlyInOut.length > 0 && (
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
            )}

            {/* Stock Movement History Table */}
            <CRow className="mb-4">
                <CCol xs={12}>
                    <CCard>
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilHistory} className="me-2 text-primary" size="lg" />
                            <strong>{t('stockMovement.history')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {!dashboard?.history || dashboard.history.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <CTable hover responsive>
                                    <CTableHead>
                                        <CTableRow>
                                            <CTableHeaderCell>{t('stockMovement.date')}</CTableHeaderCell>
                                            <CTableHeaderCell>{t('stockMovement.product')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-center">{t('stockMovement.type')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-end">{t('stockMovement.quantity')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-end">{t('stockMovement.price')}</CTableHeaderCell>
                                            <CTableHeaderCell>{t('stockMovement.order')}</CTableHeaderCell>
                                            <CTableHeaderCell>{t('stockMovement.reason')}</CTableHeaderCell>
                                        </CTableRow>
                                    </CTableHead>
                                    <CTableBody>
                                        {dashboard.history.slice(0, 20).map((movement) => (
                                            <CTableRow key={movement.id}>
                                                <CTableDataCell>{formatDate(movement.date)}</CTableDataCell>
                                                <CTableDataCell>
                                                    <a href={`/basic/products?id=${movement.productId}`} onClick={(e) => openProductModal(movement.productId, e)} className="text-decoration-none">
                                                        <div className="fw-semibold">{movement.productName}</div>
                                                    </a>
                                                </CTableDataCell>
                                                <CTableDataCell className="text-center">
                                                    <span className={`badge bg-${getTypeBadgeColor(movement.type)}`}>
                                                        {t(`stockMovement.${movement.type.toLowerCase()}`)}
                                                    </span>
                                                </CTableDataCell>
                                                <CTableDataCell className="text-end">
                                                    {formatNumber(movement.quantity)}
                                                </CTableDataCell>
                                                <CTableDataCell className="text-end">
                                                    {formatCurrency(movement.price)}
                                                </CTableDataCell>
                                                <CTableDataCell>
                                                    {movement.orderId ? (
                                                        <a href={`/basic/order/${movement.orderId}`} onClick={(e) => openOrderModal(movement.orderId, e)} className="text-primary">
                                                            {movement.orderId.substring(0, 8)}...
                                                        </a>
                                                    ) : (
                                                        '-'
                                                    )}
                                                </CTableDataCell>
                                                <CTableDataCell>
                                                    {movement.orderId ? (
                                                        <a href={`/basic/order/${movement.orderId}`} onClick={(e) => openOrderModal(movement.orderId, e)} className="text-decoration-none">
                                                            {movement.reason || '-'}
                                                        </a>
                                                    ) : (
                                                        movement.reason || '-'
                                                    )}
                                                </CTableDataCell>
                                            </CTableRow>
                                        ))}
                                    </CTableBody>
                                </CTable>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            {/* Top Moved Products and Turnover Rates */}
            <CRow xs={{ gutter: 4 }}>
                <CCol md={6}>
                    <CCard className="mb-4">
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilLayers} className="me-2" size="lg" />
                            <strong>{t('stockMovement.topMovedProducts')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {!dashboard?.topMovedProducts || dashboard.topMovedProducts.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <CTable hover responsive>
                                    <CTableHead>
                                        <CTableRow>
                                            <CTableHeaderCell>{t('stockMovement.product')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-end">{t('stockMovement.totalMoved')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-end">{t('stockMovement.movements')}</CTableHeaderCell>
                                        </CTableRow>
                                    </CTableHead>
                                    <CTableBody>
                                        {dashboard.topMovedProducts.map((product) => (
                                            <CTableRow key={product.productId}>
                                                <CTableDataCell>
                                                    <a href={`/basic/products?id=${product.productId}`} onClick={(e) => openProductModal(product.productId, e)} className="text-decoration-none">
                                                        <div className="fw-semibold">{product.productName}</div>
                                                    </a>
                                                    <small className="text-body-secondary">{product.categoryName}</small>
                                                </CTableDataCell>
                                                <CTableDataCell className="text-end">
                                                    <strong>{formatNumber(product.totalMoved)}</strong>
                                                </CTableDataCell>
                                                <CTableDataCell className="text-end">
                                                    <span className="badge bg-secondary">{product.movementCount}</span>
                                                </CTableDataCell>
                                            </CTableRow>
                                        ))}
                                    </CTableBody>
                                </CTable>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol md={6}>
                    <CCard className="mb-4">
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilSpeedometer} className="me-2" size="lg" />
                            <strong>{t('stockMovement.turnoverRates')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {!dashboard?.turnoverRates || dashboard.turnoverRates.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <CTable hover responsive>
                                    <CTableHead>
                                        <CTableRow>
                                            <CTableHeaderCell>{t('stockMovement.product')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-end">{t('stockMovement.rate')}</CTableHeaderCell>
                                            <CTableHeaderCell className="text-center">{t('stockMovement.classification')}</CTableHeaderCell>
                                        </CTableRow>
                                    </CTableHead>
                                    <CTableBody>
                                        {dashboard.turnoverRates.map((item) => (
                                            <CTableRow key={item.productId}>
                                                <CTableDataCell>
                                                    <a href={`/basic/products?id=${item.productId}`} onClick={(e) => openProductModal(item.productId, e)} className="text-decoration-none">
                                                        <div className="fw-semibold">{item.productName}</div>
                                                    </a>
                                                    <small className="text-body-secondary">{item.categoryName}</small>
                                                </CTableDataCell>
                                                <CTableDataCell className="text-end">
                                                    <strong>{item.turnoverRate.toFixed(2)}x</strong>
                                                </CTableDataCell>
                                                <CTableDataCell className="text-center">
                                                    <span className={`badge bg-${getTurnoverBadgeColor(item.turnoverClassification)}`}>
                                                        {t(`stockMovement.${item.turnoverClassification.toLowerCase().replace(/\s+/g, '')}`)}
                                                    </span>
                                                </CTableDataCell>
                                            </CTableRow>
                                        ))}
                                    </CTableBody>
                                </CTable>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            {/* Quick View Modals */}
            <ProductModal
                visible={productModalVisible}
                onClose={() => {
                    setProductModalVisible(false)
                    setSelectedItem(null)
                }}
                onSave={() => {
                    setProductModalVisible(false)
                    setSelectedItem(null)
                    loadDashboard()
                }}
                product={selectedItem}
                loading={modalLoading}
            />
        </CContainer>
    );
};

export default StockMovementDashboardView;
