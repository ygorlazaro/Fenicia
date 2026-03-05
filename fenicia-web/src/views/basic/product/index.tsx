import { cilArrowBottom, cilArrowTop, cilBan, cilChart, cilDollar, cilPencil, cilPlus, cilTrash, cilWarning } from '@coreui/icons';
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
    CTabPane
} from '@coreui/react';
import { CChartPie } from '@coreui/react-chartjs';
import { getStyle } from '@coreui/utils';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';
import Pagination from '../../../components/Pagination';
import ProductModal from '../../../components/ProductModal';
import { BasicDataSourceClient, BasicProductClient } from '../../../services/basic-crud-clients';
import ProductPerformanceClient from '../../../services/product-performance-client';

const productClient = new BasicProductClient();
const dataSourceClient = new BasicDataSourceClient();
const performanceClient = new ProductPerformanceClient();

const ProductList = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Product list state
    const [products, setProducts] = useState([]);
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
    const [selectedProduct, setSelectedProduct] = useState(null);
    const [productToDelete, setProductToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Analytics state
    const [performanceLoading, setPerformanceLoading] = useState(false);
    const [performance, setPerformance] = useState(null);

    // Data lists for modal
    const [categories, setCategories] = useState([]);
    const [suppliers, setSuppliers] = useState([]);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const productId = searchParams.get('id');
        if (productId) {
            loadProductForEdit(productId);
        }
        loadProducts();
        loadCategories();
        loadSuppliers();
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

    const loadSuppliers = async () => {
        try {
            const response = await dataSourceClient.getSuppliers();
            const data = Array.isArray(response) ? response : [];
            setSuppliers(data);
        } catch (err) {
            console.error('Failed to load suppliers:', err);
        }
    };

    const loadPerformance = async () => {
        try {
            setPerformanceLoading(true);
            const data = await performanceClient.getPerformance(analyticsDays);
            setPerformance(data);
        } catch (err) {
            console.error('Failed to load product performance:', err);
            setError(t('products.performanceLoadError'));
        } finally {
            setPerformanceLoading(false);
        }
    };

    const loadProductForEdit = async (productId) => {
        try {
            const product = await productClient.getById(productId);
            setSelectedProduct(product);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load product for edit:', err);
            setError(t('products.loadError'));
        }
    };

    const loadProducts = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await productClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const productsList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? productsList.length;
            setProducts(productsList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            setError(t('products.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedProduct(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (product) => {
        try {
            const fullProduct = await productClient.getById(product.id);
            setSelectedProduct(fullProduct);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load product details:', err);
            setError(t('products.loadError'));
        }
    };

    const handleOpenDelete = (product) => {
        setProductToDelete(product);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            if (selectedProduct) {
                await productClient.update(selectedProduct.id, formData);
                setSuccessMessage(t('products.updateSuccess'));
            } else {
                await productClient.create(formData);
                setSuccessMessage(t('products.createSuccess'));
            }
            setModalVisible(false);
            loadProducts();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('products.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!productToDelete) return;

        setDeleting(true);
        try {
            await productClient.delete(productToDelete.id);
            setSuccessMessage(t('products.deleteSuccess'));
            setDeleteModalVisible(false);
            setProductToDelete(null);
            loadProducts();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('products.loadError'));
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
        if (!value && value !== 0) return '-';
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };

    const getMarginBadgeColor = (classification) => {
        switch (classification?.toLowerCase()) {
            case 'excellent': return 'success';
            case 'good': return 'info';
            case 'average': return 'warning';
            case 'low': return 'orange';
            default: return 'danger';
        }
    };

    const getBestSellersChartData = () => {
        if (!performance || performance.bestSellingProducts.length === 0) return null;
        return {
            labels: performance.bestSellingProducts.map(p => p.productName.substring(0, 20)),
            datasets: [{
                label: t('products.quantitySold'),
                backgroundColor: getStyle('--cui-primary'),
                data: performance.bestSellingProducts.map(p => p.totalQuantitySold)
            }]
        };
    };

    const getMarginDistributionData = () => {
        if (!performance || performance.profitMargins.length === 0) return null;
        const distribution = {
            'Excellent': 0,
            'Good': 0,
            'Average': 0,
            'Low': 0,
            'Very Low': 0
        };
        performance.profitMargins.forEach(p => {
            if (distribution[p.marginClassification] !== undefined) {
                distribution[p.marginClassification]++;
            }
        });
        return {
            labels: Object.keys(distribution),
            datasets: [{
                data: Object.values(distribution),
                backgroundColor: [
                    getStyle('--cui-success'),
                    getStyle('--cui-info'),
                    getStyle('--cui-warning'),
                    getStyle('--cui-orange'),
                    getStyle('--cui-danger')
                ]
            }]
        };
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
                                {t('products.last30Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 90 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(90)}>
                                {t('products.last90Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 180 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(180)}>
                                {t('products.last180Days')}
                            </CButton>
                        </div>
                    </CCol>
                </CRow>

                {/* Best Selling Products */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilArrowTop} className="me-2 text-success" />
                                <strong>{t('products.bestSellingProducts')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.bestSellingProducts.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>#</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.category')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('products.quantitySold')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.revenue')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('products.orders')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.bestSellingProducts.map((product, index) => (
                                                <CTableRow key={product.productId}>
                                                    <CTableDataCell>{index + 1}</CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                            <strong>{product.productName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{product.categoryName}</CTableDataCell>
                                                    <CTableDataCell className="text-center">{product.totalQuantitySold}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <strong>{formatCurrency(product.totalRevenue)}</strong>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">{product.orderCount}</CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Worst Selling Products */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilArrowBottom} className="me-2 text-warning" />
                                <strong>{t('products.worstSellingProducts')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.worstSellingProducts.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.category')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('products.sold')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('products.stock')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.costValue')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.worstSellingProducts.map((product) => (
                                                <CTableRow key={product.productId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                            {product.productName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{product.categoryName}</CTableDataCell>
                                                    <CTableDataCell className="text-center">{product.totalQuantitySold}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={product.currentStock > 50 ? 'warning' : 'info'}>
                                                            {product.currentStock}
                                                        </CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <span className="text-danger">{formatCurrency(product.costValue)}</span>
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

                {/* Profit Margins */}
                <CRow className="mb-4" xs={{ gutter: 4 }}>
                    <CCol md={6}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilChart} className="me-2" />
                                <strong>{t('products.marginDistribution')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.profitMargins.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CChartPie data={getMarginDistributionData()} options={{ responsive: true, maintainAspectRatio: true }} />
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                    <CCol md={6}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilDollar} className="me-2" />
                                <strong>{t('products.profitMargins')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.profitMargins.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.cost')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.price')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('products.margin')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.profitMargins.slice(0, 10).map((product) => (
                                                <CTableRow key={product.productId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                            {product.productName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(product.costPrice)}</CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(product.salesPrice)}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={getMarginBadgeColor(product.marginClassification)}>
                                                            {product.profitMargin.toFixed(1)}% ({product.marginClassification})
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

                {/* Products Never Sold */}
                <CRow>
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilBan} className="me-2 text-danger" />
                                <strong>{t('products.neverSoldProducts')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.neverSoldProducts.length === 0 ? (
                                    <p className="text-muted text-center">{t('products.allProductsSelling')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.category')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.supplier')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('products.stock')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('products.costValue')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.neverSoldProducts.map((product) => (
                                                <CTableRow key={product.productId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                            <strong>{product.productName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{product.categoryName}</CTableDataCell>
                                                    <CTableDataCell>{product.supplierName || '-'}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color="danger">{product.currentStock}</CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <span className="text-danger">{formatCurrency(product.costValue)}</span>
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
                    <strong>{t('products.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('products.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilPencil} className="me-2" />
                                {t('products.productsList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t('products.performance')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Products List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && products.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && products.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.category')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.supplier')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.costPrice')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.salesPrice')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('products.quantity')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {products.map((product) => (
                                                <CTableRow key={product.id}>
                                                    <CTableDataCell>{product.name}</CTableDataCell>
                                                    <CTableDataCell>
                                                        {product.categoryId ? (
                                                            <Link to={`/basic/product-categories?id=${product.categoryId}`} className="text-decoration-none">
                                                                {product.categoryName || '-'}
                                                            </Link>
                                                        ) : (
                                                            '-'
                                                        )}
                                                    </CTableDataCell>
                                                    <CTableDataCell>
                                                        {product.supplierId ? (
                                                            <Link to={`/basic/suppliers?id=${product.supplierId}`} className="text-decoration-none">
                                                                {product.supplierName || '-'}
                                                            </Link>
                                                        ) : (
                                                            '-'
                                                        )}
                                                    </CTableDataCell>
                                                    <CTableDataCell>{formatCurrency(product.costPrice)}</CTableDataCell>
                                                    <CTableDataCell>{formatCurrency(product.salesPrice)}</CTableDataCell>
                                                    <CTableDataCell>{product.quantity ?? 0}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <CButton color="info" size="sm" className="me-2" onClick={() => handleOpenEdit(product)}>
                                                            <CIcon icon={cilPencil} />
                                                        </CButton>
                                                        <CButton color="danger" size="sm" onClick={() => handleOpenDelete(product)}>
                                                            <CIcon icon={cilTrash} />
                                                        </CButton>
                                                    </CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>

                                    <Pagination pagination={pagination} onPageChange={handlePageChange} onPerPageChange={handlePerPageChange} />
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

            <ProductModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                product={selectedProduct}
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
                        {t('products.deleteConfirm', { name: productToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('products.deleteWarning')}
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

export default ProductList;
