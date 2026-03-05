import { cilArrowBottom, cilChart, cilClock, cilPencil, cilPeople, cilPlus, cilTrash, cilWarning } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CAlert,
    CBadge,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CNav,
    CNavItem,
    CNavLink,
    CRow,
    CSpinner,
    CTabContent,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CTabPane,
    CWidgetStatsA
} from '@coreui/react';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';
import CustomerModal from '../../../components/CustomerModal';
import Pagination from '../../../components/Pagination';
import { BasicCustomerClient, BasicOrderClient } from '../../../services/basic-crud-clients';
import CustomerInsightsClient from '../../../services/customer-insights-client';

const customerClient = new BasicCustomerClient();
const orderClient = new BasicOrderClient();
const insightsClient = new CustomerInsightsClient();

const Customers = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Customer list state
    const [customers, setCustomers] = useState([]);
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
    const [selectedCustomer, setSelectedCustomer] = useState(null);
    const [customerToDelete, setCustomerToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Analytics state
    const [insightsLoading, setInsightsLoading] = useState(false);
    const [insights, setInsights] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const customerId = searchParams.get('id');
        if (customerId) {
            loadCustomerForEdit(customerId);
        }
        loadCustomers();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadInsights();
        }
    }, [activeTab, analyticsDays]);

    const loadInsights = async () => {
        try {
            setInsightsLoading(true);
            const data = await insightsClient.getInsights(analyticsDays);
            setInsights(data);
        } catch (err) {
            console.error('Failed to load customer insights:', err);
            setError(t('customers.insightsLoadError'));
        } finally {
            setInsightsLoading(false);
        }
    };

    const loadCustomerForEdit = async (customerId) => {
        try {
            const customer = await customerClient.getById(customerId);
            setSelectedCustomer(customer);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load customer for edit:', err);
            setError(t('customers.loadError'));
        }
    };

    const loadCustomers = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await customerClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const customersList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? customersList.length;
            setCustomers(customersList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error('Failed to load customers:', err);
            setError(t('customers.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedCustomer(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (customer) => {
        try {
            const fullCustomer = await customerClient.getById(customer.id);
            setSelectedCustomer(fullCustomer);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load customer details:', err);
            setError(t('customers.loadError'));
        }
    };

    const handleOpenDelete = (customer) => {
        setCustomerToDelete(customer);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('customers.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload = {
                id: selectedCustomer?.id || crypto.randomUUID(),
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                phoneNumber: formData.phoneNumber || null,
                street: formData.street || null,
                number: formData.number || null,
                neighborhood: formData.neighborhood || null,
                city: formData.city || null,
                complement: formData.complement || null,
                zipCode: formData.zipCode || null,
                stateId: formData.stateId || null
            };

            if (selectedCustomer) {
                await customerClient.update(selectedCustomer.id, payload);
                setSuccessMessage(t('customers.updateSuccess'));
            } else {
                await customerClient.create(payload);
                setSuccessMessage(t('customers.createSuccess'));
            }
            setModalVisible(false);
            loadCustomers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save customer:', err);
            setError(err.response?.data?.title || t('customers.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!customerToDelete) return;

        setDeleting(true);
        try {
            await customerClient.delete(customerToDelete.id);
            setSuccessMessage(t('customers.deleteSuccess'));
            setDeleteModalVisible(false);
            setCustomerToDelete(null);
            loadCustomers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete customer:', err);
            setError(t('customers.loadError'));
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

    const formatPhone = (phone) => {
        if (!phone) return '-';
        const cleaned = phone.replace(/\D/g, '');
        if (cleaned.length === 10) {
            return `(${cleaned.slice(0, 2)}) ${cleaned.slice(2, 6)}-${cleaned.slice(6)}`;
        }
        return phone;
    };

    const formatCurrency = (value) => {
        if (!value && value !== 0) return '-';
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleDateString();
    };

    // Render Analytics Tab Content
    const renderAnalyticsTab = () => {
        if (insightsLoading) {
            return (
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            );
        }

        if (!insights) {
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
                            <CButton size="sm" color={analyticsDays === 30 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(30)}>
                                {t('customers.last30Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 90 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(90)}>
                                {t('customers.last90Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 180 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(180)}>
                                {t('customers.last180Days')}
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
                                    {insights.summary.totalCustomers}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('customers.customers')}
                                    </span>
                                </>
                            }
                            title={t('customers.totalCustomers')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="success"
                            value={
                                <>
                                    {insights.summary.totalOrders}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('customers.orders')}
                                    </span>
                                </>
                            }
                            title={t('customers.totalOrders')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="info"
                            value={
                                <>
                                    {formatCurrency(insights.summary.totalRevenue)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('customers.revenue')}
                                    </span>
                                </>
                            }
                            title={t('customers.totalRevenue')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="warning"
                            value={
                                <>
                                    {formatCurrency(insights.summary.averageOrderValue)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('customers.aov')}
                                    </span>
                                </>
                            }
                            title={t('customers.averageOrderValue')}
                        />
                    </CCol>
                </CRow>

                {/* Top Customers */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilPeople} className="me-2" />
                                <strong>{t('customers.topCustomers')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {insights.topCustomers.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>#</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.customer')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.orders')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('customers.totalSpent')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('customers.aov')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.lastOrder')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {insights.topCustomers.map((customer, index) => (
                                                <CTableRow key={customer.customerId}>
                                                    <CTableDataCell>{index + 1}</CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/customers?id=${customer.customerId}`} className="text-decoration-none">
                                                            <strong>{customer.customerName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{customer.orderCount}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <strong>{formatCurrency(customer.totalSpent)}</strong>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(customer.averageOrderValue)}</CTableDataCell>
                                                    <CTableDataCell className="text-center">{formatDate(customer.lastOrderDate)}</CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Recent Orders */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilClock} className="me-2" />
                                <strong>{t('customers.recentOrders')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {insights.recentOrders.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('customers.date')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.customer')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('customers.totalAmount')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.status')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.items')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {insights.recentOrders.map((order) => (
                                                <CTableRow key={order.orderId}>
                                                    <CTableDataCell>{formatDate(order.saleDate)}</CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/order/${order.orderId}`} className="text-decoration-none">
                                                            {order.customerName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <strong>{formatCurrency(order.totalAmount)}</strong>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={order.status === 'Approved' ? 'success' : order.status === 'Pending' ? 'warning' : 'danger'}>
                                                            {t(`orders.statusValues.${order.status.toLowerCase()}`)}
                                                        </CBadge>
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

                {/* At-Risk Customers */}
                <CRow>
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilArrowBottom} className="me-2 text-danger" />
                                <strong>{t('customers.atRiskCustomers')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {insights.atRiskCustomers.length === 0 ? (
                                    <p className="text-muted text-center">{t('customers.noAtRiskCustomers')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('customers.customer')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.previousOrders')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.daysSinceLastOrder')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('customers.previousTotalSpent')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('customers.riskLevel')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {insights.atRiskCustomers.map((customer) => (
                                                <CTableRow key={customer.customerId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/customers?id=${customer.customerId}`} className="text-decoration-none">
                                                            <strong>{customer.customerName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{customer.previousOrderCount}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={customer.daysSinceLastOrder >= 120 ? 'danger' : customer.daysSinceLastOrder >= 90 ? 'warning' : 'info'}>
                                                            {customer.daysSinceLastOrder} {t('customers.days')}
                                                        </CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(customer.previousTotalSpent)}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={customer.riskLevel === 'High' ? 'danger' : customer.riskLevel === 'Medium' ? 'warning' : 'info'}>
                                                            {t(`customers.${customer.riskLevel.toLowerCase()}`)}
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
                    <strong>{t('customers.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('customers.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilPeople} className="me-2" />
                                {t('customers.customersList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t('customers.insights')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Customers List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && customers.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && customers.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('customers.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.email')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.phone')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('customers.document')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {customers.map((customer) => (
                                                <CTableRow key={customer.id}>
                                                    <CTableDataCell>{customer.name}</CTableDataCell>
                                                    <CTableDataCell>{customer.email}</CTableDataCell>
                                                    <CTableDataCell>{formatPhone(customer.phoneNumber)}</CTableDataCell>
                                                    <CTableDataCell>{customer.document || '-'}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <CButton
                                                            color="info"
                                                            size="sm"
                                                            className="me-2"
                                                            onClick={() => handleOpenEdit(customer)}
                                                        >
                                                            <CIcon icon={cilPencil} />
                                                        </CButton>
                                                        <CButton
                                                            color="danger"
                                                            size="sm"
                                                            onClick={() => handleOpenDelete(customer)}
                                                        >
                                                            <CIcon icon={cilTrash} />
                                                        </CButton>
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

            <CustomerModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                customer={selectedCustomer}
                loading={saving}
            />

            <CModal
                visible={deleteModalVisible}
                onClose={() => setDeleteModalVisible(false)}
            >
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        {t('common.confirmDelete')}
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>
                        {t('customers.deleteConfirm', { name: customerToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('customers.deleteWarning')}
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

export default Customers;
