import React, { useEffect, useState, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import {
    CButton,
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
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CSpinner,
    CAlert,
    CForm,
    CFormInput,
    CFormLabel,
    CFormSelect,
    CRow,
    CCol,
    CNav,
    CNavItem,
    CNavLink,
    CTabContent,
    CTabPane,
    CWidgetStatsA,
    CProgress,
    CListGroup,
    CListGroupItem
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilPlus, cilTrash, cilWarning, cilCalendar, cilUser, cilCart, cilChart, cilPeople, cilDollar, cilBan } from '@coreui/icons';
import { CChartPie, CChartLine, CChartBar } from '@coreui/react-chartjs';
import { BasicOrderClient, BasicDataSourceClient } from '../../../services/basic-crud-clients';
import OrderAnalyticsClient from '../../../services/order-analytics-client';
import Pagination from '../../../components/Pagination';
import { getStyle } from '@coreui/utils';

const orderClient = new BasicOrderClient();
const dataSourceClient = new BasicDataSourceClient();
const analyticsClient = new OrderAnalyticsClient();

const Orders = () => {
    const { t } = useTranslation();
    
    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Order list state
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [pagination, setPagination] = useState({
        page: 1,
        perPage: 10,
        total: 0,
        pages: 0
    });

    // Modal state
    const [modalVisible, setModalVisible] = useState(false);
    const [deleteModalVisible, setDeleteModalVisible] = useState(false);
    const [orderToDelete, setOrderToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Form state - Step 1 (Header)
    const [customerId, setCustomerId] = useState('');
    const [saleDate, setSaleDate] = useState(new Date().toISOString().split('T')[0]);
    const [status, setStatus] = useState('Pending');
    const [employeeId, setEmployeeId] = useState('');

    // Form state - Step 2 (Details)
    const [orderItems, setOrderItems] = useState([]);
    const [selectedProduct, setSelectedProduct] = useState('');
    const [quantity, setQuantity] = useState(1);
    const [price, setPrice] = useState(0);

    // Data lists
    const [products, setProducts] = useState([]);
    const [customers, setCustomers] = useState([]);
    const [employees, setEmployees] = useState([]);
    const [isAdmin, setIsAdmin] = useState(false);

    // Analytics state
    const [analyticsLoading, setAnalyticsLoading] = useState(false);
    const [analytics, setAnalytics] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        loadProducts();
        loadCustomers();
        loadEmployees();
        checkAdminRole();
    }, []);

    useEffect(() => {
        loadOrders();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadAnalytics();
        }
    }, [activeTab, analyticsDays]);

    const checkAdminRole = () => {
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        const roles = user.roles || [];
        setIsAdmin(roles.includes('Admin'));
    };

    const loadProducts = async () => {
        try {
            const response = await dataSourceClient.getProducts();
            const data = Array.isArray(response) ? response : [];
            setProducts(data);
        } catch (err) {
            console.error('Failed to load products:', err);
        }
    };

    const loadCustomers = async () => {
        try {
            const response = await dataSourceClient.getCustomers();
            const data = Array.isArray(response) ? response : [];
            setCustomers(data);
        } catch (err) {
            console.error('Failed to load customers:', err);
        }
    };

    const loadEmployees = async () => {
        try {
            const response = await dataSourceClient.getEmployees();
            const data = Array.isArray(response) ? response : [];
            setEmployees(data);
        } catch (err) {
            console.error('Failed to load employees:', err);
        }
    };

    const loadOrders = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await orderClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const ordersList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? ordersList.length;
            setOrders(ordersList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error('Failed to load orders:', err);
            setError(t('orders.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const loadAnalytics = async () => {
        try {
            setAnalyticsLoading(true);
            const data = await analyticsClient.getAnalytics(analyticsDays);
            setAnalytics(data);
        } catch (err) {
            console.error('Failed to load analytics:', err);
            setError(t('orders.analyticsLoadError'));
        } finally {
            setAnalyticsLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setCustomerId('');
        setSaleDate(new Date().toISOString().split('T')[0]);
        setStatus('Pending');
        setEmployeeId('');
        setOrderItems([]);
        setSelectedProduct('');
        setQuantity(1);
        setPrice(0);
        setActiveTab(0);
        setModalVisible(true);
    };

    const handleOpenDelete = (order) => {
        setOrderToDelete(order);
        setDeleteModalVisible(true);
    };

    const handleProductChange = (productId: string) => {
        setSelectedProduct(productId);
        const product = products.find(p => p.id === productId);
        if (product) {
            setPrice(product.salesPrice || 0);
        }
    };

    const handleAddItem = () => {
        if (!selectedProduct || quantity <= 0 || price <= 0) {
            setError(t('common.requiredField'));
            return;
        }

        const product = products.find(p => p.id === selectedProduct);
        if (!product) return;

        const existingItem = orderItems.find(item => item.productId === selectedProduct);
        if (existingItem) {
            setOrderItems(orderItems.map(item =>
                item.productId === selectedProduct
                    ? { ...item, quantity: item.quantity + Number(quantity) }
                    : item
            ));
        } else {
            setOrderItems([...orderItems, {
                productId: selectedProduct,
                productName: product.name,
                price: Number(product.salesPrice || price),
                quantity: Number(quantity)
            }]);
        }

        setSelectedProduct('');
        setQuantity(1);
        setPrice(0);
    };

    const handleRemoveItem = (productId) => {
        setOrderItems(orderItems.filter(item => item.productId !== productId));
    };

    const handleNextTab = () => {
        if (!customerId || !saleDate || !status) {
            setError(t('common.requiredField'));
            return;
        }
        setActiveTab(1);
    };

    const handlePrevTab = () => {
        setActiveTab(0);
    };

    const handleSave = async (e) => {
        e.preventDefault();

        if (!customerId || !saleDate || !status) {
            setError(t('common.requiredField'));
            return;
        }

        if (orderItems.length === 0) {
            setError(t('orders.items') + ' ' + t('common.requiredField'));
            return;
        }

        setSaving(true);
        try {
            const payload = {
                customerId: customerId,
                saleDate: new Date(saleDate).toISOString(),
                status: status,
                employeeId: employeeId || null,
                details: orderItems.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity,
                    price: item.price
                }))
            };

            await orderClient.create(payload);
            setSuccessMessage(t('orders.createSuccess'));
            setModalVisible(false);
            loadOrders();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to create order:', err);
            setError(err.response?.data?.title || t('orders.loadError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!orderToDelete) return;

        setDeleting(true);
        try {
            await orderClient.delete(orderToDelete.id);
            setSuccessMessage(t('orders.deleteSuccess'));
            setDeleteModalVisible(false);
            setOrderToDelete(null);
            loadOrders();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete order:', err);
            setError(t('orders.loadError'));
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (newPage) => {
        setPagination(prev => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage) => {
        setPagination(prev => ({ ...prev, perPage: newPerPage, page: 1 }));
    };

    const formatCurrency = (value) => {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleDateString();
    };

    const getStatusBadgeColor = (status) => {
        switch (status?.toLowerCase()) {
            case 'pending':
                return 'warning';
            case 'approved':
                return 'success';
            case 'cancelled':
                return 'danger';
            default:
                return 'secondary';
        }
    };

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

    // Render Analytics Tab Content
    const renderAnalyticsTab = () => {
        if (analyticsLoading) {
            return (
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            );
        }

        if (!analytics) {
            return (
                <div className="text-center py-5">
                    <p className="text-muted">{t('common.noData')}</p>
                </div>
            );
        }

        return (
            <>
                {/* Time Range Selector */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <div className="d-flex justify-content-end gap-2">
                            <CButton
                                size="sm"
                                color={analyticsDays === 30 ? 'primary' : 'outline-primary'}
                                onClick={() => setAnalyticsDays(30)}
                            >
                                {t('orders.last30Days')}
                            </CButton>
                            <CButton
                                size="sm"
                                color={analyticsDays === 90 ? 'primary' : 'outline-primary'}
                                onClick={() => setAnalyticsDays(90)}
                            >
                                {t('orders.last90Days')}
                            </CButton>
                            <CButton
                                size="sm"
                                color={analyticsDays === 180 ? 'primary' : 'outline-primary'}
                                onClick={() => setAnalyticsDays(180)}
                            >
                                {t('orders.last180Days')}
                            </CButton>
                        </div>
                    </CCol>
                </CRow>

                {/* Summary Cards */}
                <CRow className="mb-4" xs={{ gutter: 4 }}>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="primary"
                            value={
                                <>
                                    {formatCurrency(analytics.averageOrderValue.averageValue)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('orders.avgOrderValue')}
                                    </span>
                                </>
                            }
                            title={t('orders.averageOrderValue')}
                        />
                    </CCol>

                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="success"
                            value={
                                <>
                                    {analytics.averageOrderValue.totalOrders}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('orders.totalOrders')}
                                    </span>
                                </>
                            }
                            title={t('orders.totalOrders')}
                        />
                    </CCol>

                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="info"
                            value={
                                <>
                                    {formatCurrency(analytics.averageOrderValue.medianValue)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('orders.medianValue')}
                                    </span>
                                </>
                            }
                            title={t('orders.medianValue')}
                        />
                    </CCol>

                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="warning"
                            value={
                                <>
                                    {analytics.cancelledOrders.length}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('orders.cancelled')}
                                    </span>
                                </>
                            }
                            title={t('orders.cancelledOrders')}
                        />
                    </CCol>
                </CRow>

                {/* Charts Row */}
                <CRow className="mb-4" xs={{ gutter: 4 }}>
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
                                        }}
                                    />
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
                                        }}
                                    />
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Top Customers */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilPeople} className="me-2" />
                                <strong>{t('orders.topCustomers')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {analytics.topCustomers.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('orders.customer')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('orders.orders')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('orders.totalSpent')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('orders.items')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {analytics.topCustomers.map((customer) => (
                                                <CTableRow key={customer.customerId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/customers?id=${customer.customerId}`} className="text-decoration-none">
                                                            <strong>{customer.customerName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{customer.orderCount}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <strong>{formatCurrency(customer.totalSpent)}</strong>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">{customer.totalItems}</CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Cancelled Orders */}
                <CRow>
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilBan} className="me-2 text-danger" />
                                <strong>{t('orders.cancelledOrdersReport')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {analytics.cancelledOrders.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('orders.date')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('orders.customer')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('orders.totalAmount')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('orders.items')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {analytics.cancelledOrders.map((order) => (
                                                <CTableRow key={order.orderId}>
                                                    <CTableDataCell>{formatDate(order.saleDate)}</CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/order/${order.orderId}`} className="text-decoration-none">
                                                            {order.customerName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <span className="text-danger">{formatCurrency(order.totalAmount)}</span>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{order.totalItems}</CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>
            </>
        );
    };

    return (
        <CContainer className="py-4">
            {error && (
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
                </CAlert>
            )}

            {successMessage && (
                <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>
                    {successMessage}
                </CAlert>
            )}

            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center">
                    <strong>{t('orders.title')}</strong>
                    <div className="d-flex gap-2">
                        <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                            <CIcon icon={cilPlus} className="me-2" />
                            {t('orders.new')}
                        </CButton>
                    </div>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink
                                active={activeTab === 0}
                                onClick={() => setActiveTab(0)}
                                style={{ cursor: 'pointer' }}
                            >
                                <CIcon icon={cilCart} className="me-2" />
                                {t('orders.ordersList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink
                                active={activeTab === 1}
                                onClick={() => setActiveTab(1)}
                                style={{ cursor: 'pointer' }}
                            >
                                <CIcon icon={cilChart} className="me-2" />
                                {t('orders.analytics')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Orders List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && orders.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && orders.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('orders.id')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('orders.customer')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('orders.total')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('orders.date')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('orders.status')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('orders.items')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {orders.map((order) => (
                                                <CTableRow key={order.id}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/order/${order.id}`} className="text-decoration-none font-monospace">
                                                            {order.id.substring(0, 8)}...
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/order/${order.id}`} className="text-decoration-none">
                                                            {order.customerName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{formatCurrency(order.totalAmount)}</CTableDataCell>
                                                    <CTableDataCell>{formatDate(order.saleDate)}</CTableDataCell>
                                                    <CTableDataCell>
                                                        <span className={`badge bg-${getStatusBadgeColor(order.status)}`}>
                                                            {t(`orders.statusValues.${order.status?.toLowerCase()}`) || order.status}
                                                        </span>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{order.totalItems}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        {isAdmin && (
                                                            <CButton
                                                                color="danger"
                                                                size="sm"
                                                                onClick={() => handleOpenDelete(order)}
                                                            >
                                                                <CIcon icon={cilTrash} />
                                                            </CButton>
                                                        )}
                                                    </CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>

                                    <Pagination
                                        pagination={pagination}
                                        onPageChange={handlePageChange}
                                        onPerPageChange={handlePerPageChange}
                                    />
                                </>
                            )}
                        </CTabPane>

                        {/* Analytics Tab */}
                        <CTabPane visible={activeTab === 1}>
                            {renderAnalyticsTab()}
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            {/* Create Order Modal */}
            <CModal visible={modalVisible} onClose={() => setModalVisible(false)} size="xl">
                <CModalHeader>
                    <CModalTitle>{t('orders.new')}</CModalTitle>
                </CModalHeader>
                <CForm onSubmit={handleSave}>
                    <CModalBody>
                        <CNav variant="tabs">
                            <CNavItem>
                                <CNavLink
                                    active={activeTab === 0}
                                    onClick={() => setActiveTab(0)}
                                    style={{ cursor: 'pointer' }}
                                >
                                    <CIcon icon={cilUser} className="me-2" />
                                    {t('orders.header')}
                                </CNavLink>
                            </CNavItem>
                            <CNavItem>
                                <CNavLink
                                    active={activeTab === 1}
                                    onClick={() => setActiveTab(1)}
                                    style={{ cursor: 'pointer' }}
                                >
                                    <CIcon icon={cilCart} className="me-2" />
                                    {t('orders.details')}
                                </CNavLink>
                            </CNavItem>
                        </CNav>

                        <CTabContent className="mt-3">
                            <CTabPane visible={activeTab === 0}>
                                <CRow>
                                    <CCol md={6}>
                                        <CFormLabel htmlFor="customer">{t('orders.customer')} *</CFormLabel>
                                        <CFormSelect
                                            id="customer"
                                            value={customerId}
                                            onChange={(e) => setCustomerId(e.target.value)}
                                            required
                                        >
                                            <option value="">{t('common.select')}</option>
                                            {customers.map(c => (
                                                <option key={c.id} value={c.id}>
                                                    {c.name}
                                                </option>
                                            ))}
                                        </CFormSelect>
                                    </CCol>
                                    <CCol md={6}>
                                        <CFormLabel htmlFor="saleDate">{t('orders.date')} *</CFormLabel>
                                        <CFormInput
                                            type="date"
                                            id="saleDate"
                                            value={saleDate}
                                            onChange={(e) => setSaleDate(e.target.value)}
                                            required
                                        />
                                    </CCol>
                                    <CCol md={12}>
                                        <CFormLabel htmlFor="status">{t('orders.statusLabel')} *</CFormLabel>
                                        <CFormSelect
                                            id="status"
                                            value={status}
                                            onChange={(e) => setStatus(e.target.value)}
                                            required
                                        >
                                            <option value="Pending">{t('orders.statusValues.pending')}</option>
                                            <option value="Approved">{t('orders.statusValues.approved')}</option>
                                            <option value="Cancelled">{t('orders.statusValues.cancelled')}</option>
                                        </CFormSelect>
                                    </CCol>
                                    <CCol md={12}>
                                        <CFormLabel htmlFor="employee">{t('orders.employee')}</CFormLabel>
                                        <CFormSelect
                                            id="employee"
                                            value={employeeId}
                                            onChange={(e) => setEmployeeId(e.target.value)}
                                        >
                                            <option value="">{t('orders.noEmployee')}</option>
                                            {employees.map(e => (
                                                <option key={e.id} value={e.id}>
                                                    {e.name}
                                                </option>
                                            ))}
                                        </CFormSelect>
                                    </CCol>
                                </CRow>
                            </CTabPane>

                            <CTabPane visible={activeTab === 1}>
                                <h6 className="mb-3">{t('orders.addItems')}</h6>

                                <CRow className="mb-4">
                                    <CCol md={5}>
                                        <CFormLabel htmlFor="product">{t('products.title')} *</CFormLabel>
                                        <CFormSelect
                                            id="product"
                                            value={selectedProduct}
                                            onChange={(e) => handleProductChange(e.target.value)}
                                        >
                                            <option value="">{t('common.select')}</option>
                                            {products.map(p => (
                                                <option key={p.id} value={p.id}>
                                                    {p.name} - {formatCurrency(p.salesPrice || 0)}
                                                </option>
                                            ))}
                                        </CFormSelect>
                                    </CCol>
                                    <CCol md={2}>
                                        <CFormLabel htmlFor="price">{t('products.price')} *</CFormLabel>
                                        <CFormInput
                                            type="number"
                                            min="0.01"
                                            step="0.01"
                                            id="price"
                                            value={price}
                                            onChange={(e) => setPrice(e.target.value)}
                                            required
                                        />
                                    </CCol>
                                    <CCol md={2}>
                                        <CFormLabel htmlFor="quantity">{t('orders.quantity')} *</CFormLabel>
                                        <CFormInput
                                            type="number"
                                            min="1"
                                            step="1"
                                            id="quantity"
                                            value={quantity}
                                            onChange={(e) => setQuantity(e.target.value)}
                                            required
                                        />
                                    </CCol>
                                    <CCol md={3} className="d-flex align-items-end">
                                        <CButton
                                            color="primary"
                                            onClick={handleAddItem}
                                            disabled={!selectedProduct}
                                        >
                                            <CIcon icon={cilPlus} className="me-2" />
                                            {t('common.add')}
                                        </CButton>
                                    </CCol>
                                </CRow>

                                <h6 className="mb-3">{t('orders.items')}</h6>

                                {orderItems.length === 0 ? (
                                    <div className="text-center py-4">
                                        <p className="text-muted">{t('orders.noItems')}</p>
                                    </div>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.price')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('orders.quantity')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('orders.subtotal')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {orderItems.map((item) => (
                                                <CTableRow key={item.productId}>
                                                    <CTableDataCell>{item.productName}</CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(item.price)}</CTableDataCell>
                                                    <CTableDataCell className="text-end">{item.quantity}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        {formatCurrency(item.price * item.quantity)}
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <CButton
                                                            color="danger"
                                                            size="sm"
                                                            onClick={() => handleRemoveItem(item.productId)}
                                                        >
                                                            <CIcon icon={cilTrash} />
                                                        </CButton>
                                                    </CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                        <CTableFoot>
                                            <CTableRow>
                                                <CTableHeaderCell colSpan={3} className="text-end">
                                                    {t('orders.total')}:
                                                </CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">
                                                    <strong>{formatCurrency(orderItems.reduce((sum, item) => sum + (item.price * item.quantity), 0))}</strong>
                                                </CTableHeaderCell>
                                                <CTableHeaderCell></CTableHeaderCell>
                                            </CTableRow>
                                        </CTableFoot>
                                    </CTable>
                                )}
                            </CTabPane>
                        </CTabContent>
                    </CModalBody>
                    <CModalFooter>
                        {activeTab === 0 ? (
                            <>
                                <CButton color="secondary" onClick={() => setModalVisible(false)} disabled={saving}>
                                    {t('common.cancel')}
                                </CButton>
                                <CButton color="primary" onClick={handleNextTab}>
                                    {t('common.next')}
                                </CButton>
                            </>
                        ) : (
                            <>
                                <CButton color="secondary" onClick={handlePrevTab} disabled={saving}>
                                    {t('common.previous')}
                                </CButton>
                                <CButton
                                    color="primary"
                                    type="submit"
                                    disabled={saving || orderItems.length === 0}
                                >
                                    {saving ? t('common.saving') : t('orders.create')}
                                </CButton>
                            </>
                        )}
                    </CModalFooter>
                </CForm>
            </CModal>

            {/* Delete Confirmation Modal */}
            <CModal visible={deleteModalVisible} onClose={() => setDeleteModalVisible(false)}>
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        {t('common.confirmDelete')}
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>
                        {t('orders.deleteConfirm', { customer: orderToDelete?.customerName })}
                    </p>
                    <p className="text-danger">
                        {t('orders.deleteWarning')}
                    </p>
                </CModalBody>
                <CModalFooter>
                    <CButton
                        color="secondary"
                        onClick={() => setDeleteModalVisible(false)}
                        disabled={deleting}
                    >
                        {t('common.cancel')}
                    </CButton>
                    <CButton
                        color="danger"
                        onClick={handleDelete}
                        disabled={deleting}
                    >
                        {deleting ? t('common.deleting') : t('common.delete')}
                    </CButton>
                </CModalFooter>
            </CModal>
        </CContainer>
    );
};

export default Orders;
