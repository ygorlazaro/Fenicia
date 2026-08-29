import { cilChart, cilPencil, cilPlus, cilTrash, cilWarning } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CAlert, CButton, CCard, CCardBody, CCardHeader, CContainer, CModal, CModalBody, CModalFooter, CModalHeader, CModalTitle, CNav, CNavItem, CNavLink, CSpinner, CTabContent, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow, CTabPane } from "@coreui/react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useSearchParams } from "react-router-dom";
import Pagination from "../../../components/fenicia/pagination";
import { BasicDataSourceClient } from "../../../services/basic/basic-datasource-client";
import { BasicProductClient } from "../../../services/basic/basic-product-client";
import { ProductPerformance } from "../../../types/basic/product/product-performance";
import formatCurrency from "../../../utils/format-currency";
import RenderAnalyticsTab from "./performance";
import ProductModal from "./product-modal";

const productClient = new BasicProductClient();
const dataSourceClient = new BasicDataSourceClient();

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
    const [performance, setPerformance] = useState<ProductPerformance | null>(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const productId = searchParams.get("id");
        if (productId) {
            loadProductForEdit(productId);
        }
        loadProducts();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadPerformance();
        }
    }, [activeTab, analyticsDays]);

    const loadPerformance = async () => {
        try {
            setPerformanceLoading(true);
            const data = await productClient.getPerformance(analyticsDays);
            setPerformance(data);
        } catch (err) {
            console.error("Failed to load product performance:", err);
            setError(t("products.performanceLoadError"));
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
            console.error("Failed to load product for edit:", err);
            setError(t("products.loadError"));
        }
    };

    const loadProducts = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await productClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const productsList = isPaginated ? response.data : Array.isArray(response) ? response : [];
            const totalItems = response?.total ?? productsList.length;
            setProducts(productsList);
            setPagination((prev) => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            setError(t("products.loadError"));
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
            console.error("Failed to load product details:", err);
            setError(t("products.loadError"));
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
                setSuccessMessage(t("products.updateSuccess"));
            } else {
                await productClient.create(formData);
                setSuccessMessage(t("products.createSuccess"));
            }
            setModalVisible(false);
            loadProducts();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t("products.saveError"));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!productToDelete) return;

        setDeleting(true);
        try {
            await productClient.delete(productToDelete.id);
            setSuccessMessage(t("products.deleteSuccess"));
            setDeleteModalVisible(false);
            setProductToDelete(null);
            loadProducts();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t("products.loadError"));
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (newPage) => {
        setPagination((prev) => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage) => {
        setPagination((prev) => ({ ...prev, perPage: newPerPage, page: 1 }));
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
                    <strong>{t("products.title")}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t("products.new")}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: "pointer" }}>
                                <CIcon icon={cilPencil} className="me-2" />
                                {t("products.productsList")}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: "pointer" }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t("products.performance")}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Products List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t("common.loading")}</p>
                                </div>
                            )}

                            {!loading && products.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t("common.noData")}</p>
                                </div>
                            )}

                            {!loading && products.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t("products.name")}</CTableHeaderCell>
                                                <CTableHeaderCell>{t("products.category")}</CTableHeaderCell>
                                                <CTableHeaderCell>{t("products.supplier")}</CTableHeaderCell>
                                                <CTableHeaderCell>{t("products.costPrice")}</CTableHeaderCell>
                                                <CTableHeaderCell>{t("products.salesPrice")}</CTableHeaderCell>
                                                <CTableHeaderCell>{t("products.quantity")}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t("common.actions")}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {products.map((product) => (
                                                <CTableRow key={product.id}>
                                                    <CTableDataCell>{product.name}</CTableDataCell>
                                                    <CTableDataCell>
                                                        {product.categoryId ? (
                                                            <Link to={`/basic/product-categories?id=${product.categoryId}`} className="text-decoration-none">
                                                                {product.categoryName || "-"}
                                                            </Link>
                                                        ) : (
                                                            "-"
                                                        )}
                                                    </CTableDataCell>
                                                    <CTableDataCell>
                                                        {product.supplierId ? (
                                                            <Link to={`/basic/suppliers?id=${product.supplierId}`} className="text-decoration-none">
                                                                {product.supplierName || "-"}
                                                            </Link>
                                                        ) : (
                                                            "-"
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
                            <RenderAnalyticsTab performance={performance} performanceLoading={performanceLoading} analyticsDays={analyticsDays} setAnalyticsDays={setAnalyticsDays} />
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            <ProductModal visible={modalVisible} onClose={() => setModalVisible(false)} onSave={handleSave} product={selectedProduct} loading={saving} />

            <CModal visible={deleteModalVisible} onClose={() => setDeleteModalVisible(false)}>
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        {t("common.confirmDelete")}
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>{t("products.deleteConfirm", { name: productToDelete?.name })}</p>
                    <p className="text-danger">{t("products.deleteWarning")}</p>
                </CModalBody>
                <CModalFooter>
                    <CButton color="secondary" onClick={() => setDeleteModalVisible(false)} disabled={deleting}>
                        {t("common.cancel")}
                    </CButton>
                    <CButton color="danger" onClick={handleDelete} disabled={deleting}>
                        {deleting ? t("common.deleting") : t("common.delete")}
                    </CButton>
                </CModalFooter>
            </CModal>
        </CContainer>
    );
};

export default ProductList;
