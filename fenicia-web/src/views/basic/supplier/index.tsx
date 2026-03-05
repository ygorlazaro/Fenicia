import { cilChart, cilDollar, cilPencil, cilPlus, cilTrash, cilTruck, cilWarning } from '@coreui/icons';
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
import Pagination from '../../../components/Pagination';
import SupplierModal from '../../../components/SupplierModal';
import { BasicDataSourceClient, BasicSupplierClient } from '../../../services/basic-crud-clients';
import SupplierPerformanceClient from '../../../services/supplier-performance-client';

const supplierClient = new BasicSupplierClient();
const dataSourceClient = new BasicDataSourceClient();
const performanceClient = new SupplierPerformanceClient();

const Suppliers = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Supplier list state
    const [suppliers, setSuppliers] = useState([]);
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
    const [selectedSupplier, setSelectedSupplier] = useState(null);
    const [supplierToDelete, setSupplierToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Analytics state
    const [performanceLoading, setPerformanceLoading] = useState(false);
    const [performance, setPerformance] = useState(null);

    // Data lists for modal
    const [categories, setCategories] = useState([]);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const supplierId = searchParams.get('id');
        if (supplierId) {
            loadSupplierForEdit(supplierId);
        }
        loadSuppliers();
        loadCategories();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadPerformance();
        }
    }, [activeTab, analyticsDays]);

    const loadCategories = async () => {
        try {
            const response = await dataSourceClient.getProductCategories();
            const data = Array.isArray(response) ? response : [];
            setCategories(data);
        } catch (err) {
            console.error('Failed to load categories:', err);
        }
    };

    const loadPerformance = async () => {
        try {
            setPerformanceLoading(true);
            const data = await performanceClient.getPerformance(analyticsDays);
            setPerformance(data);
        } catch (err) {
            console.error('Failed to load supplier performance:', err);
            setError(t('suppliers.performanceLoadError'));
        } finally {
            setPerformanceLoading(false);
        }
    };

    const loadSupplierForEdit = async (supplierId) => {
        try {
            const supplier = await supplierClient.getById(supplierId);
            setSelectedSupplier(supplier);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load supplier for edit:', err);
            setError(t('suppliers.loadError'));
        }
    };

    const loadSuppliers = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await supplierClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const suppliersList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? suppliersList.length;
            setSuppliers(suppliersList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error('Failed to load suppliers:', err);
            setError(t('suppliers.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedSupplier(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (supplier) => {
        try {
            const fullSupplier = await supplierClient.getById(supplier.id);
            setSelectedSupplier(fullSupplier);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load supplier details:', err);
            setError(t('suppliers.loadError'));
        }
    };

    const handleOpenDelete = (supplier) => {
        setSupplierToDelete(supplier);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        setError(null);

        if (!formData.name || !formData.email) {
            setError(t('suppliers.requiredFields'));
            setSaving(false);
            return;
        }

        try {
            const payload = {
                id: selectedSupplier?.id || crypto.randomUUID(),
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

            if (selectedSupplier) {
                await supplierClient.update(selectedSupplier.id, payload);
                setSuccessMessage(t('suppliers.updateSuccess'));
            } else {
                await supplierClient.create(payload);
                setSuccessMessage(t('suppliers.createSuccess'));
            }
            setModalVisible(false);
            loadSuppliers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to save supplier:', err);
            setError(err.response?.data?.title || t('suppliers.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!supplierToDelete) return;

        setDeleting(true);
        try {
            await supplierClient.delete(supplierToDelete.id);
            setSuccessMessage(t('suppliers.deleteSuccess'));
            setDeleteModalVisible(false);
            setSupplierToDelete(null);
            loadSuppliers();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error('Failed to delete supplier:', err);
            setError(t('suppliers.loadError'));
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

    // Render Analytics Tab Content
    const renderAnalyticsTab = () => {
        if (performanceLoading) {
            return (
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            );
        }

        if (!performance) {
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
                                {t('suppliers.last30Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 90 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(90)}>
                                {t('suppliers.last90Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 180 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(180)}>
                                {t('suppliers.last180Days')}
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
                                    {performance.summary.totalSuppliers}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('suppliers.suppliers')}
                                    </span>
                                </>
                            }
                            title={t('suppliers.totalSuppliers')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="success"
                            value={
                                <>
                                    {performance.summary.totalProducts}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('suppliers.products')}
                                    </span>
                                </>
                            }
                            title={t('suppliers.totalProducts')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="info"
                            value={
                                <>
                                    {formatCurrency(performance.summary.totalStockValue)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('suppliers.stockValue')}
                                    </span>
                                </>
                            }
                            title={t('suppliers.totalStockValue')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="warning"
                            value={
                                <>
                                    {performance.summary.averageProductsPerSupplier.toFixed(1)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('suppliers.avgPerSupplier')}
                                    </span>
                                </>
                            }
                            title={t('suppliers.averageProductsPerSupplier')}
                        />
                    </CCol>
                </CRow>

                {/* Products per Supplier */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilTruck} className="me-2" />
                                <strong>{t('suppliers.productsPerSupplier')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.productsPerSupplier.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('suppliers.name')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('suppliers.products')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('suppliers.stockValue')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('suppliers.revenue')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.productsPerSupplier.map((supplier, index) => (
                                                <CTableRow key={supplier.supplierId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/suppliers?id=${supplier.supplierId}`} className="text-decoration-none">
                                                            <strong>{supplier.supplierName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{supplier.productCount}</CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(supplier.totalStockValue)}</CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(supplier.totalRevenue)}</CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Cost Comparison */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilDollar} className="me-2" />
                                <strong>{t('suppliers.costComparison')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.costComparison.length === 0 ? (
                                    <p className="text-muted text-center">{t('suppliers.noCostComparison')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.supplier')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.costPrice')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.salesPrice')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('suppliers.margin')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.costComparison.map((product) => (
                                                product.suppliers.map((supplier, idx) => (
                                                    <CTableRow key={`${product.productName}-${supplier.supplierId}`}>
                                                        <CTableDataCell rowSpan={product.suppliers.length}>{product.productName}</CTableDataCell>
                                                        <CTableDataCell>
                                                            <Link to={`/basic/suppliers?id=${supplier.supplierId}`} className="text-decoration-none">
                                                                {supplier.supplierName}
                                                            </Link>
                                                        </CTableDataCell>
                                                        <CTableDataCell className="text-end">{formatCurrency(supplier.costPrice)}</CTableDataCell>
                                                        <CTableDataCell className="text-end">{formatCurrency(supplier.salesPrice)}</CTableDataCell>
                                                        <CTableDataCell className="text-center">
                                                            <CBadge color={supplier.profitMargin >= 30 ? 'success' : supplier.profitMargin >= 15 ? 'warning' : 'danger'}>
                                                                {supplier.profitMargin.toFixed(1)}%
                                                            </CBadge>
                                                        </CTableDataCell>
                                                    </CTableRow>
                                                ))
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Recent Stock Movements */}
                <CRow>
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilChart} className="me-2" />
                                <strong>{t('suppliers.recentMovements')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.recentStockMovements.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('suppliers.date')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('suppliers.type')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.quantity')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.price')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.recentStockMovements.map((movement) => (
                                                <CTableRow key={movement.movementId}>
                                                    <CTableDataCell>{new Date(movement.date).toLocaleDateString()}</CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${movement.productId}`} className="text-decoration-none">
                                                            {movement.productName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={movement.movementType === 'In' ? 'success' : 'danger'}>
                                                            {t(`suppliers.${movement.movementType.toLowerCase()}`)}
                                                        </CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">{movement.quantity}</CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(movement.price)}</CTableDataCell>
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
                    <strong>{t('suppliers.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('suppliers.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilTruck} className="me-2" />
                                {t('suppliers.suppliersList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t('suppliers.performance')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Suppliers List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && suppliers.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && suppliers.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('suppliers.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.email')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.phone')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('suppliers.document')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {suppliers.map((supplier) => (
                                                <CTableRow key={supplier.id}>
                                                    <CTableDataCell>{supplier.name}</CTableDataCell>
                                                    <CTableDataCell>{supplier.email}</CTableDataCell>
                                                    <CTableDataCell>{formatPhone(supplier.phoneNumber)}</CTableDataCell>
                                                    <CTableDataCell>{supplier.document || '-'}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <CButton
                                                            color="info"
                                                            size="sm"
                                                            className="me-2"
                                                            onClick={() => handleOpenEdit(supplier)}
                                                        >
                                                            <CIcon icon={cilPencil} />
                                                        </CButton>
                                                        <CButton
                                                            color="danger"
                                                            size="sm"
                                                            onClick={() => handleOpenDelete(supplier)}
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

            <SupplierModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                supplier={selectedSupplier}
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
                        {t('suppliers.deleteConfirm', { name: supplierToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('suppliers.deleteWarning')}
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

export default Suppliers;
