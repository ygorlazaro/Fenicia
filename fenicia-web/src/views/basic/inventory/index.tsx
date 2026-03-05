import {
    cilArrowBottom,
    cilArrowTop,
    cilDollar,
    cilLayers,
    cilPeople,
    cilTags,
    cilTruck,
    cilWarning
} from '@coreui/icons'
import CIcon from '@coreui/icons-react'
import {
    CAlert,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CListGroup,
    CListGroupItem,
    CProgress,
    CRow,
    CSpinner,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CWidgetStatsA
} from '@coreui/react'
import { CChartBar, CChartDoughnut, CChartLine } from '@coreui/react-chartjs'
import { getStyle } from '@coreui/utils'
import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'
import InventoryClient from '../../../services/inventory-client'

const inventoryClient = new InventoryClient()

interface LowStockItem {
    id: string
    name: string
    quantity: number
    costPrice: number | null
    salesPrice: number
    categoryId: string
    categoryName: string
}

interface CategoryBreakdown {
    categoryId: string
    categoryName: string
    totalCostValue: number
    totalSalesValue: number
    totalQuantity: number
}

interface SupplierBreakdown {
    supplierId: string
    supplierName: string
    totalCostValue: number
    totalSalesValue: number
    totalQuantity: number
}

interface DashboardData {
    lowStockItems: LowStockItem[]
    totalCustomers: number
    totalEmployees: number
    totalCostValue: number
    totalSalesValue: number
    totalQuantity: number
    profitPotential: number
    categoryBreakdown: CategoryBreakdown[]
    supplierBreakdown: SupplierBreakdown[]
}

const InventoryDashboard = () => {
    const { t } = useTranslation()
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [dashboard, setDashboard] = useState<DashboardData | null>(null)

    useEffect(() => {
        loadDashboard()
    }, [])

    const loadDashboard = async () => {
        try {
            setLoading(true)
            setError(null)
            const data = await inventoryClient.getDashboard()
            setDashboard(data)
        } catch (err) {
            setError(t('inventory.loadError'))
            console.error('Failed to load inventory dashboard:', err)
        } finally {
            setLoading(false)
        }
    }

    const formatCurrency = (value: number) => {
        if (value === null || value === undefined) return '-'
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
        }).format(value)
    }

    const formatNumber = (value: number) => {
        return new Intl.NumberFormat('pt-BR').format(value)
    }

    const calculateProfitMargin = () => {
        if (!dashboard || dashboard.totalSalesValue === 0) return 0
        return ((dashboard.profitPotential / dashboard.totalSalesValue) * 100).toFixed(1)
    }

    const getStockLevel = (quantity: number) => {
        if (quantity === 0) return { color: 'danger', label: t('inventory.outOfStock') }
        if (quantity < 10) return { color: 'warning', label: t('inventory.lowStock') }
        return { color: 'success', label: t('inventory.inStock') }
    }

    const getCategoryChartData = () => {
        if (!dashboard || dashboard.categoryBreakdown.length === 0) return null

        return {
            labels: dashboard.categoryBreakdown.map(c => c.categoryName),
            datasets: [
                {
                    label: t('inventory.costValue'),
                    backgroundColor: getStyle('--cui-danger'),
                    data: dashboard.categoryBreakdown.map(c => c.totalCostValue),
                },
                {
                    label: t('inventory.salesValue'),
                    backgroundColor: getStyle('--cui-success'),
                    data: dashboard.categoryBreakdown.map(c => c.totalSalesValue),
                },
            ],
        }
    }

    if (loading) {
        return (
            <CContainer className="py-4">
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            </CContainer>
        )
    }

    if (error) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
                </CAlert>
            </CContainer>
        )
    }

    return (
        <CContainer className="py-4">
            {/* Financial Metrics Cards - Using CWidgetStatsA */}
            <CRow className="mb-4" xs={{ gutter: 4 }}>
                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="primary"
                        value={
                            <>
                                {formatCurrency(dashboard?.totalCostValue ?? 0)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {formatNumber(dashboard?.totalQuantity ?? 0)} {t('inventory.items')}
                                </span>
                            </>
                        }
                        title={t('inventory.totalCostValue')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilDollar} size="xl" className="text-white-50" />
                            </div>
                        }
                        chart={
                            <CChartLine
                                className="mt-3 mx-3"
                                style={{ height: '70px' }}
                                data={{
                                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                                    datasets: [
                                        {
                                            label: 'Cost',
                                            backgroundColor: 'transparent',
                                            borderColor: 'rgba(255,255,255,.55)',
                                            pointBackgroundColor: getStyle('--cui-primary'),
                                            data: [65, 59, 84, 84, 51, 55, dashboard?.totalCostValue ?? 40],
                                        },
                                    ],
                                }}
                                options={{
                                    plugins: { legend: { display: false } },
                                    maintainAspectRatio: false,
                                    scales: {
                                        x: { border: { display: false }, grid: { display: false }, ticks: { display: false } },
                                        y: { display: false, grid: { display: false }, ticks: { display: false } },
                                    },
                                    elements: { line: { borderWidth: 1, tension: 0.4 }, point: { radius: 4 } },
                                }}
                            />
                        }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="success"
                        value={
                            <>
                                {formatCurrency(dashboard?.totalSalesValue ?? 0)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    +{calculateProfitMargin()}% {t('inventory.margin')}
                                </span>
                            </>
                        }
                        title={t('inventory.totalSalesValue')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilTruck} size="xl" className="text-white-50" />
                            </div>
                        }
                        chart={
                            <CChartLine
                                className="mt-3 mx-3"
                                style={{ height: '70px' }}
                                data={{
                                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                                    datasets: [
                                        {
                                            label: 'Sales',
                                            backgroundColor: 'transparent',
                                            borderColor: 'rgba(255,255,255,.55)',
                                            pointBackgroundColor: getStyle('--cui-success'),
                                            data: [1, 18, 9, 17, 34, 22, dashboard?.totalSalesValue ?? 11],
                                        },
                                    ],
                                }}
                                options={{
                                    plugins: { legend: { display: false } },
                                    maintainAspectRatio: false,
                                    scales: {
                                        x: { border: { display: false }, grid: { display: false }, ticks: { display: false } },
                                        y: { display: false, grid: { display: false }, ticks: { display: false } },
                                    },
                                    elements: { line: { borderWidth: 1 }, point: { radius: 4 } },
                                }}
                            />
                        }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="warning"
                        value={
                            <>
                                {formatCurrency(dashboard?.profitPotential ?? 0)}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {dashboard?.profitPotential >= 0 ? (
                                        <>
                                            <CIcon icon={cilArrowTop} /> {t('inventory.profit')}
                                        </>
                                    ) : (
                                        <>
                                            <CIcon icon={cilArrowBottom} /> {t('inventory.loss')}
                                        </>
                                    )}
                                </span>
                            </>
                        }
                        title={t('inventory.profitPotential')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilDollar} size="xl" className="text-white-50" />
                            </div>
                        }
                        chart={
                            <CChartBar
                                className="mt-3 mx-3"
                                style={{ height: '70px' }}
                                data={{
                                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                                    datasets: [
                                        {
                                            label: 'Profit',
                                            backgroundColor: 'rgba(255,255,255,.2)',
                                            borderColor: 'rgba(255,255,255,.55)',
                                            data: [78, 81, 80, 45, 34, 12, 40, 85, 65, 23, 12, dashboard?.profitPotential ?? 82],
                                            barPercentage: 0.6,
                                        },
                                    ],
                                }}
                                options={{
                                    maintainAspectRatio: false,
                                    plugins: { legend: { display: false } },
                                    scales: {
                                        x: { grid: { display: false, drawTicks: false }, ticks: { display: false } },
                                        y: { border: { display: false }, grid: { display: false }, ticks: { display: false } },
                                    },
                                }}
                            />
                        }
                    />
                </CCol>

                <CCol sm={6} xl={3}>
                    <CWidgetStatsA
                        color="info"
                        value={
                            <>
                                {dashboard?.totalCustomers ?? 0}
                                <span className="fs-6 fw-normal d-block mt-1">
                                    {dashboard?.totalEmployees ?? 0} {t('inventory.employees')}
                                </span>
                            </>
                        }
                        title={t('inventory.customersAndEmployees')}
                        action={
                            <div className="mt-2">
                                <CIcon icon={cilPeople} size="xl" className="text-white-50" />
                            </div>
                        }
                        chart={
                            <CChartDoughnut
                                className="mx-3"
                                style={{ height: '70px' }}
                                data={{
                                    labels: [t('inventory.customers'), t('inventory.employees')],
                                    datasets: [
                                        {
                                            backgroundColor: [getStyle('--cui-info'), getStyle('--cui-warning')],
                                            data: [dashboard?.totalCustomers ?? 70, dashboard?.totalEmployees ?? 30],
                                        },
                                    ],
                                }}
                                options={{
                                    plugins: { legend: { display: false } },
                                    maintainAspectRatio: false,
                                }}
                            />
                        }
                    />
                </CCol>
            </CRow>

            {/* Category and Supplier Breakdown - Using WidgetsBrand style */}
            <CRow className="mb-4">
                <CCol md={6}>
                    <CCard className="mb-4">
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilLayers} className="me-2" size="lg" />
                            <strong>{t('inventory.breakdownByCategory')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {dashboard?.categoryBreakdown.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <CListGroup flush>
                                    {dashboard?.categoryBreakdown.slice(0, 5).map((category, index) => (
                                        <CListGroupItem
                                            key={category.categoryId}
                                            className="d-flex justify-content-between align-items-center"
                                        >
                                            <div>
                                                <Link to={`/basic/product-categories?id=${category.categoryId}`} className="text-decoration-none">
                                                    <div className="fw-semibold">{category.categoryName}</div>
                                                </Link>
                                                <small className="text-body-secondary">
                                                    {formatNumber(category.totalQuantity)} {t('inventory.items')}
                                                </small>
                                            </div>
                                            <div className="text-end">
                                                <div className="text-success fw-semibold">
                                                    {formatCurrency(category.totalSalesValue)}
                                                </div>
                                                <small className="text-body-secondary">
                                                    {formatCurrency(category.totalCostValue)} {t('inventory.cost')}
                                                </small>
                                            </div>
                                        </CListGroupItem>
                                    ))}
                                </CListGroup>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>

                <CCol md={6}>
                    <CCard className="mb-4">
                        <CCardHeader className="d-flex align-items-center">
                            <CIcon icon={cilTruck} className="me-2" size="lg" />
                            <strong>{t('inventory.breakdownBySupplier')}</strong>
                        </CCardHeader>
                        <CCardBody>
                            {dashboard?.supplierBreakdown.length === 0 ? (
                                <p className="text-muted text-center">{t('common.noData')}</p>
                            ) : (
                                <CListGroup flush>
                                    {dashboard?.supplierBreakdown.slice(0, 5).map((supplier, index) => (
                                        <CListGroupItem
                                            key={supplier.supplierId}
                                            className="d-flex justify-content-between align-items-center"
                                        >
                                            <div>
                                                <Link to={`/basic/suppliers?id=${supplier.supplierId}`} className="text-decoration-none">
                                                    <div className="fw-semibold">{supplier.supplierName}</div>
                                                </Link>
                                                <small className="text-body-secondary">
                                                    {formatNumber(supplier.totalQuantity)} {t('inventory.items')}
                                                </small>
                                            </div>
                                            <div className="text-end">
                                                <div className="text-success fw-semibold">
                                                    {formatCurrency(supplier.totalSalesValue)}
                                                </div>
                                                <small className="text-body-secondary">
                                                    {formatCurrency(supplier.totalCostValue)} {t('inventory.cost')}
                                                </small>
                                            </div>
                                        </CListGroupItem>
                                    ))}
                                </CListGroup>
                            )}
                        </CCardBody>
                    </CCard>
                </CCol>
            </CRow>

            {/* Category Chart */}
            {dashboard?.categoryBreakdown && dashboard.categoryBreakdown.length > 0 && (
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader>
                                <CIcon icon={cilTags} className="me-2" />
                                <strong>{t('inventory.categoryComparison')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                <CChartBar
                                    data={getCategoryChartData()}
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
                                                ticks: {
                                                    callback: (value) => `R$ ${Number(value) / 1000}k`,
                                                },
                                            },
                                        },
                                    }}
                                />
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>
            )}

            {/* Low Stock Items Table */}
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilWarning} className="me-2 text-warning" size="lg" />
                    <strong>{t('inventory.lowStockItems')}</strong>
                </CCardHeader>
                <CCardBody>
                    {dashboard?.lowStockItems.length === 0 ? (
                        <div className="text-center py-4">
                            <p className="text-muted">{t('common.noData')}</p>
                        </div>
                    ) : (
                            <CTable align="middle" className="mb-0 border" hover responsive>
                            <CTableHead>
                                <CTableRow>
                                        <CTableHeaderCell className="bg-body-tertiary">
                                            {t('inventory.productName')}
                                        </CTableHeaderCell>
                                        <CTableHeaderCell className="bg-body-tertiary text-center">
                                            {t('inventory.category')}
                                        </CTableHeaderCell>
                                        <CTableHeaderCell className="bg-body-tertiary text-center">
                                            {t('inventory.quantity')}
                                        </CTableHeaderCell>
                                        <CTableHeaderCell className="bg-body-tertiary text-end">
                                            {t('inventory.costPrice')}
                                        </CTableHeaderCell>
                                        <CTableHeaderCell className="bg-body-tertiary text-end">
                                            {t('inventory.salesPrice')}
                                        </CTableHeaderCell>
                                        <CTableHeaderCell className="bg-body-tertiary text-center">
                                            {t('inventory.status')}
                                        </CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                    {dashboard?.lowStockItems.map((item) => {
                                        const stockLevel = getStockLevel(item.quantity)
                                    return (
                                        <CTableRow key={item.id}>
                                            <CTableDataCell>
                                                <Link to={`/basic/products?id=${item.id}`} className="text-decoration-none">
                                                    <div className="fw-semibold">{item.name}</div>
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <Link to={`/basic/product-categories?id=${item.categoryId}`} className="text-decoration-none">
                                                    {item.categoryName}
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <div className="d-flex align-items-center justify-content-center">
                                                    <CIcon icon={cilPeople} className="me-2" size="sm" />
                                                    <span className="fw-semibold">{item.quantity}</span>
                                                </div>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                {formatCurrency(item.costPrice ?? 0)}
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                {formatCurrency(item.salesPrice)}
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <CProgress
                                                    thin
                                                    color={stockLevel.color}
                                                    value={Math.min(item.quantity * 10, 100)}
                                                    style={{ width: '60px', display: 'inline-block' }}
                                                />
                                                <div className="small text-body-secondary mt-1">{stockLevel.label}</div>
                                            </CTableDataCell>
                                        </CTableRow>
                                    )
                                })}
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>
        </CContainer>
    )
}

export default InventoryDashboard
