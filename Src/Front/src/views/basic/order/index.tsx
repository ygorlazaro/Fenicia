import { cilCart, cilChart, cilPlus, cilWarning } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CAlert, CButton, CCard, CCardBody, CCardHeader, CContainer, CModal, CModalBody, CModalFooter, CModalHeader, CModalTitle, CNav, CNavItem, CNavLink, CTabContent, CTabPane } from "@coreui/react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { BasicOrderClient } from "../../../services/basic/basic-order-client";
import { OrderAnalytics } from "../../../types/basic/order/order-analytics";
import { CreateOrderCommand, GetAllOrderResponse } from "../../../types/basic/product-category/add-product-category-command";
import CreateOrderModal from "./modal";
import OrderTable from "./order-table";
import RenderAnalyticsTab from "./performance";

const orderClient = new BasicOrderClient();

const Orders = () => {
    const { t } = useTranslation();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    // Order list state
    const [orders, setOrders] = useState<GetAllOrderResponse[]>([]);
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
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Analytics state
    const [analyticsLoading, setAnalyticsLoading] = useState(false);
    const [analytics, setAnalytics] = useState<OrderAnalytics | null>(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        loadOrders();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadAnalytics();
        }
    }, [activeTab, analyticsDays]);

    const loadOrders = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await orderClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const ordersList = isPaginated ? response.data : Array.isArray(response) ? response : [];
            const totalItems = response?.total ?? ordersList.length;
            setOrders(ordersList);
            setPagination((prev) => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            console.error("Failed to load orders:", err);
            setError(t("orders.loadError"));
        } finally {
            setLoading(false);
        }
    };

    const loadAnalytics = async () => {
        try {
            setAnalyticsLoading(true);
            const data = await orderClient.getAnalytics(analyticsDays);
            setAnalytics(data);
        } catch (err) {
            console.error("Failed to load analytics:", err);
            setError(t("orders.analyticsLoadError"));
        } finally {
            setAnalyticsLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setActiveTab(0);
        setModalVisible(true);
    };

    const handleOpenDelete = (order: GetAllOrderResponse) => {
        setOrderToDelete(order);
        setDeleteModalVisible(true);
    };

    const handleSave = async (e, payload: CreateOrderCommand) => {
        e.preventDefault();

        try {
            await orderClient.create(payload);
            setSuccessMessage(t("orders.createSuccess"));
            setModalVisible(false);
            loadOrders();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error("Failed to create order:", err);
            setError(err.response?.data?.title || t("orders.loadError"));
        }
    };

    const handleDelete = async () => {
        if (!orderToDelete) return;

        setDeleting(true);
        try {
            await orderClient.delete(orderToDelete.id);
            setSuccessMessage(t("orders.deleteSuccess"));
            setDeleteModalVisible(false);
            setOrderToDelete(null);
            loadOrders();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            console.error("Failed to delete order:", err);
            setError(t("orders.loadError"));
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (newPage: number) => {
        setPagination((prev) => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage: number) => {
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
                    <strong>{t("orders.title")}</strong>
                    <div className="d-flex gap-2">
                        <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                            <CIcon icon={cilPlus} className="me-2" />
                            {t("orders.new")}
                        </CButton>
                    </div>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: "pointer" }}>
                                <CIcon icon={cilCart} className="me-2" />
                                {t("orders.ordersList")}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: "pointer" }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t("orders.analytics")}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Orders List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            <OrderTable loading={loading} orders={orders} handlePageChange={handlePageChange} handlePerPageChange={handlePerPageChange} handleOpenDelete={handleOpenDelete} />
                        </CTabPane>

                        {/* Analytics Tab */}
                        <CTabPane visible={activeTab === 1}>
                            <RenderAnalyticsTab analytics={analytics} analyticsDays={analyticsDays} setAnalyticsDays={setAnalyticsDays} analyticsLoading={analyticsLoading} />
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            {/* Create Order Modal */}
            <CreateOrderModal modalVisible={modalVisible} setModalVisible={setModalVisible} handleSave={handleSave} setError={setError} />

            {/* Delete Confirmation Modal */}
            <CModal visible={deleteModalVisible} onClose={() => setDeleteModalVisible(false)}>
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        {t("common.confirmDelete")}
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>{t("orders.deleteConfirm", { customer: orderToDelete?.customerName })}</p>
                    <p className="text-danger">{t("orders.deleteWarning")}</p>
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

export default Orders;
