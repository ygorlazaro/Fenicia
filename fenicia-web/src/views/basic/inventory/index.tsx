import {
    cilArrowBottom,
    cilArrowTop,
    cilDollar,
    cilLayers,
    cilPeople,
    cilTags,
    cilTruck,
    cilWarning,
    cilChart,
    cilBan,
    cilPuzzle
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
    CWidgetStatsA,
    CWidgetStatsB,
    CBadge,
    CNav,
    CNavItem,
    CNavLink,
    CTabContent,
    CTabPane
} from '@coreui/react'
import { CChartBar, CChartDoughnut, CChartLine } from '@coreui/react-chartjs'
import { getStyle } from '@coreui/utils'
import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router-dom'
import InventoryClient from '../../../services/inventory-client'
import InventoryHealthClient from '../../../services/inventory-health-client'
import { BasicProductClient, BasicProductCategoryClient, BasicSupplierClient } from '../../../services/basic-crud-clients'
import ProductModal from '../../../components/ProductModal'
import ProductCategoryModal from '../../../components/ProductCategoryModal'
import SupplierModal from '../../../components/SupplierModal'

const inventoryClient = new InventoryClient()
const healthClient = new InventoryHealthClient()

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
    const navigate = useNavigate()
    
    // Tab state
    const [activeTab, setActiveTab] = useState(0)
    
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [dashboard, setDashboard] = useState<DashboardData | null>(null)
    
    // Health analytics state
    const [healthLoading, setHealthLoading] = useState(false)
    const [health, setHealth] = useState(null)
    const [zeroMovementDays, setZeroMovementDays] = useState(90)
    const [overstockMultiplier, setOverstockMultiplier] = useState(3.0)

    // Modal state for quick view without navigation
    const [productModalVisible, setProductModalVisible] = useState(false)
    const [categoryModalVisible, setCategoryModalVisible] = useState(false)
    const [supplierModalVisible, setSupplierModalVisible] = useState(false)
    const [selectedItem, setSelectedItem] = useState(null)
    const [modalLoading, setModalLoading] = useState(false)

    const productClient = new BasicProductClient()
    const categoryClient = new BasicProductCategoryClient()
    const supplierClient = new BasicSupplierClient()

    useEffect(() => {
        loadDashboard()
    }, [])

    useEffect(() => {
        if (activeTab === 1) {
            loadHealth()
        }
    }, [activeTab, zeroMovementDays, overstockMultiplier])

    const loadHealth = async () => {
        try {
            setHealthLoading(true)
            const data = await healthClient.getInventoryHealth(zeroMovementDays, overstockMultiplier)
            setHealth(data)
        } catch (err) {
            setError(t('inventory.healthLoadError'))
            console.error('Failed to load inventory health:', err)
        } finally {
            setHealthLoading(false)
        }
    }

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

    const openCategoryModal = async (categoryId: string, e: React.MouseEvent) => {
        e.preventDefault()
        e.stopPropagation()
        try {
            setModalLoading(true)
            const category = await categoryClient.getById(categoryId)
            setSelectedItem(category)
            setCategoryModalVisible(true)
        } catch (err) {
            console.error('Failed to load category:', err)
            navigate(`/basic/product-categories?id=${categoryId}`)
        } finally {
            setModalLoading(false)
        }
    }

    const openSupplierModal = async (supplierId: string, e: React.MouseEvent) => {
        e.preventDefault()
        e.stopPropagation()
        try {
            setModalLoading(true)
            const supplier = await supplierClient.getById(supplierId)
            setSelectedItem(supplier)
            setSupplierModalVisible(true)
        } catch (err) {
            console.error('Failed to load supplier:', err)
            navigate(`/basic/suppliers?id=${supplierId}`)
        } finally {
            setModalLoading(false)
        }
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

    // Render Health Tab Content
    const renderHealthTab = () => {
        if (healthLoading) {
            return (
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            )
        }

        if (!health) {
            return (
                <div className="text-center py-5">
                    <p className="text-muted">{t('common.noData')}</p>
                </div>
            )
        }

        return (
            <>
                {/* Summary Cards */}
                <CRow className="mb-4" xs={{ gutter: 4 }}>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsB
                            color="success"
                            title={t('inventory.healthyProducts')}
                            value={
                                <>
                                    {health.summary.healthyProducts}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {formatPercentage(100 - health.summary.overstockPercentage - health.summary.zeroMovementPercentage)} {t('inventory.ofTotal')}
                                    </span>
                                </>
                            }
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsB
                            color="warning"
                            title={t('inventory.overstockProducts')}
                            value={
                                <>
                                    {health.summary.overstockProducts}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {formatCurrency(health.overstockAlert.totalOverstockValue)} {t('inventory.tiedUp')}
                                    </span>
                                </>
                            }
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsB
                            color="danger"
                            title={t('inventory.zeroMovementProducts')}
                            value={
                                <>
                                    {health.zeroMovementProducts.length}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {formatCurrency(health.zeroMovementProducts.reduce((sum, p) => sum + p.stockValue, 0))} {t('inventory.atRisk')}
                                    </span>
                                </>
                            }
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsB
                            color="info"
                            title={t('inventory.totalStockValue')}
                            value={
                                <>
                                    {formatCurrency(health.summary.totalStockValue)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {health.summary.totalProducts} {t('inventory.products')}
                                    </span>
                                </>
                            }
                        />
                    </CCol>
                </CRow>

                {/* Overstock Alert */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilWarning} className="me-2 text-warning" />
                                <strong>{t('inventory.overstockAlert')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {health.overstockAlert.products.length === 0 ? (
                                    <p className="text-muted text-center">{t('inventory.noOverstock')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('inventory.product')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('inventory.category')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('inventory.currentQty')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('inventory.recommendedQty')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('inventory.excessValue')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {health.overstockAlert.products.map((product) => (
                                                <CTableRow key={product.productId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                            <strong>{product.productName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{product.categoryName}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color="danger">{product.currentQuantity}</CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{product.recommendedQuantity.toFixed(0)}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <span className="text-danger">{formatCurrency(product.excessValue)}</span>
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

                {/* Zero Movement Products */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilBan} className="me-2 text-danger" />
                                <strong>{t('inventory.zeroMovementProducts')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {health.zeroMovementProducts.length === 0 ? (
                                    <p className="text-muted text-center">{t('inventory.allProductsMoving')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('inventory.product')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('inventory.category')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('inventory.stock')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('inventory.stockValue')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('inventory.daysNoMovement')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {health.zeroMovementProducts.map((product) => (
                                                <CTableRow key={product.productId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                            <strong>{product.productName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{product.categoryName}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color="warning">{product.currentStock}</CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <span className="text-danger">{formatCurrency(product.stockValue)}</span>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={product.daysWithoutMovement >= 180 ? 'danger' : product.daysWithoutMovement >= 120 ? 'warning' : 'info'}>
                                                            {product.daysWithoutMovement} {t('inventory.days')}
                                                        </CBadge>
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

                {/* Stock Value by Category */}
                <CRow>
                    <CCol md={6}>
                        <CCard className="mb-4">
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilPuzzle} className="me-2" />
                                <strong>{t('inventory.stockValueByCategory')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {health.stockValueByCategory.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CChartDoughnut
                                        data={{
                                            labels: health.stockValueByCategory.map(c => c.categoryName),
                                            datasets: [{
                                                data: health.stockValueByCategory.map(c => c.totalStockValue),
                                                backgroundColor: [
                                                    getStyle('--cui-primary'),
                                                    getStyle('--cui-success'),
                                                    getStyle('--cui-info'),
                                                    getStyle('--cui-warning'),
                                                    getStyle('--cui-danger'),
                                                    getStyle('--cui-secondary'),
                                                ]
                                            }]
                                        }}
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
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                    <CCol md={6}>
                        <CCard className="mb-4">
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilLayers} className="me-2" />
                                <strong>{t('inventory.categoryBreakdown')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {health.stockValueByCategory.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CListGroup flush>
                                        {health.stockValueByCategory.map((category) => (
                                            <CListGroupItem
                                                key={category.categoryId}
                                                className="d-flex justify-content-between align-items-center"
                                            >
                                                <div>
                                                    <div className="fw-semibold">{category.categoryName}</div>
                                                    <small className="text-body-secondary">
                                                        {category.productCount} {t('inventory.products')}
                                                    </small>
                                                </div>
                                                <div className="text-end">
                                                    <div className="text-success fw-semibold">
                                                        {formatCurrency(category.totalStockValue)}
                                                    </div>
                                                    <small className="text-body-secondary">
                                                        {formatPercentage(category.percentage)}
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
            </>
        )
    }

    const formatPercentage = (value: number) => {
        return `${value.toFixed(1)}%`
    }

    return (
        <CContainer className="py-4">
            <CCard>
                <CCardHeader>
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink
                                active={activeTab === 0}
                                onClick={() => setActiveTab(0)}
                                style={{ cursor: 'pointer' }}
                            >
                                <CIcon icon={cilChart} className="me-2" />
                                {t('inventory.dashboard')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink
                                active={activeTab === 1}
                                onClick={() => setActiveTab(1)}
                                style={{ cursor: 'pointer' }}
                            >
                                <CIcon icon={cilWarning} className="me-2" />
                                {t('inventory.health')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>
                </CCardHeader>
                <CCardBody>
                    <CTabContent>
                        <CTabPane visible={activeTab === 0}>
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
                                                <a href={`/basic/products?id=${item.id}`} onClick={(e) => openProductModal(item.id, e)} className="text-decoration-none">
                                                    <div className="fw-semibold">{item.name}</div>
                                                </a>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <a href={`/basic/product-categories?id=${item.categoryId}`} onClick={(e) => openCategoryModal(item.categoryId, e)} className="text-decoration-none">
                                                    {item.categoryName}
                                                </a>
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
                        </CTabPane>
                        <CTabPane visible={activeTab === 1}>
                            {renderHealthTab()}
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

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

            <ProductCategoryModal
                visible={categoryModalVisible}
                onClose={() => {
                    setCategoryModalVisible(false)
                    setSelectedItem(null)
                }}
                onSave={() => {
                    setCategoryModalVisible(false)
                    setSelectedItem(null)
                    loadDashboard()
                }}
                category={selectedItem}
                loading={modalLoading}
            />

            <SupplierModal
                visible={supplierModalVisible}
                onClose={() => {
                    setSupplierModalVisible(false)
                    setSelectedItem(null)
                }}
                onSave={() => {
                    setSupplierModalVisible(false)
                    setSelectedItem(null)
                    loadDashboard()
                }}
                supplier={selectedItem}
                loading={modalLoading}
            />
        </CContainer>
    )
}

export default InventoryDashboard
