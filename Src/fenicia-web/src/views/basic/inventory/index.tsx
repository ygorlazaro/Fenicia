import { cilChart, cilWarning } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CAlert, CCard, CCardBody, CCardHeader, CCol, CContainer, CNav, CNavItem, CNavLink, CRow, CSpinner, CTabContent, CTabPane } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import BasicInventoryClient from "../../../services/basic/basic-inventory-client";
import { BasicProductCategoryClient } from "../../../services/basic/basic-product-category-client";
import { BasicProductClient } from "../../../services/basic/basic-product-client";
import { DashboardData } from "../../../types/basic/inventory/dashboard-data";
import { InventoryHealth } from "../../../types/basic/inventory/inventory-health";
import ProductCategoryModal from "../product-category/product-category-modal";
import ProductModal from "../product/product-modal";
import SupplierModal from "../supplier/supplier-modal";
import BreakdownByCategory from "./breakdown-by-category";
import BreakdownBySupplier from "./breakdown-by-supplier";
import CategoryChart from "./category-chart";
import RenderHealthTab from "./health";
import LowStockItemsTable from "./low-stock-items.table";
import ProfitPotential from "./profit-potential";
import TotalCostValue from "./total-cost-value";
import TotalCustomers from "./total-customers";
import TotalSalesValue from "./total-sales-value";

const inventoryClient = new BasicInventoryClient();
const productClient = new BasicProductClient();
const categoryClient = new BasicProductCategoryClient();

const InventoryDashboard = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [dashboard, setDashboard] = useState<DashboardData | null>(null);

    // Health analytics state
    const [healthLoading, setHealthLoading] = useState(false);
    const [health, setHealth] = useState<InventoryHealth | null>(null);
    const [zeroMovementDays, setZeroMovementDays] = useState(90);
    const [overstockMultiplier, setOverstockMultiplier] = useState(3.0);

    // Modal state for quick view without navigation
    const [productModalVisible, setProductModalVisible] = useState(false);
    const [categoryModalVisible, setCategoryModalVisible] = useState(false);
    const [supplierModalVisible, setSupplierModalVisible] = useState(false);
    const [selectedItem, setSelectedItem] = useState(null);
    const [modalLoading, setModalLoading] = useState(false);

    useEffect(() => {
        loadDashboard();
    }, []);

    useEffect(() => {
        if (activeTab === 1) {
            loadHealth();
        }
    }, [activeTab, zeroMovementDays, overstockMultiplier]);

    const loadHealth = async () => {
        try {
            setHealthLoading(true);
            const data = await inventoryClient.getInventoryHealth(zeroMovementDays, overstockMultiplier);
            setHealth(data);
        } catch (err) {
            setError(t("inventory.healthLoadError"));
            console.error("Failed to load inventory health:", err);
        } finally {
            setHealthLoading(false);
        }
    };

    const loadDashboard = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await inventoryClient.getDashboard();
            setDashboard(data);
        } catch (err) {
            setError(t("inventory.loadError"));
            console.error("Failed to load inventory dashboard:", err);
        } finally {
            setLoading(false);
        }
    };

    // Open modal without navigation
    const openProductModal = async (productId: string, e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        try {
            setModalLoading(true);
            const product = await productClient.getById(productId);
            setSelectedItem(product);
            setProductModalVisible(true);
        } catch (err) {
            console.error("Failed to load product:", err);
            navigate(`/basic/products?id=${productId}`);
        } finally {
            setModalLoading(false);
        }
    };

    const openCategoryModal = async (categoryId: string, e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        try {
            setModalLoading(true);
            const category = await categoryClient.getById(categoryId);
            setSelectedItem(category);
            setCategoryModalVisible(true);
        } catch (err) {
            console.error("Failed to load category:", err);
            navigate(`/basic/product-categories?id=${categoryId}`);
        } finally {
            setModalLoading(false);
        }
    };

    if (loading) {
        return (
            <CContainer className="py-4">
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t("common.loading")}</p>
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
            <CCard>
                <CCardHeader>
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: "pointer" }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t("inventory.dashboard")}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: "pointer" }}>
                                <CIcon icon={cilWarning} className="me-2" />
                                {t("inventory.health")}
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
                                    <TotalCostValue totalCostValue={dashboard.totalCostValue} totalQuantity={dashboard.totalQuantity} />
                                </CCol>

                                <CCol sm={6} xl={3}>
                                    <TotalSalesValue totalSalesValue={dashboard.totalSalesValue} profitPotential={dashboard.profitPotential} />
                                </CCol>

                                <CCol sm={6} xl={3}>
                                    <ProfitPotential profitPotential={dashboard.profitPotential} />
                                </CCol>

                                <CCol sm={6} xl={3}>
                                    <TotalCustomers totalCustomers={dashboard.totalCustomers} totalEmployees={dashboard.totalEmployees} />
                                </CCol>
                            </CRow>

                            {/* Category and Supplier Breakdown - Using WidgetsBrand style */}
                            <CRow className="mb-4">
                                <CCol md={6}>
                                    <BreakdownByCategory data={dashboard.categoryBreakdown} />
                                </CCol>

                                <CCol md={6}>
                                    <BreakdownBySupplier data={dashboard.supplierBreakdown} />
                                </CCol>
                            </CRow>

                            {dashboard?.categoryBreakdown && dashboard.categoryBreakdown.length > 0 && <CategoryChart data={dashboard.categoryBreakdown} />}

                            <LowStockItemsTable items={dashboard.lowStockItems} onProductClick={openProductModal} onCategoryClick={openCategoryModal} />
                        </CTabPane>
                        <CTabPane visible={activeTab === 1}>
                            <RenderHealthTab health={health} healthLoading={healthLoading} />
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            {/* Quick View Modals */}
            <ProductModal
                visible={productModalVisible}
                onClose={() => {
                    setProductModalVisible(false);
                    setSelectedItem(null);
                }}
                onSave={() => {
                    setProductModalVisible(false);
                    setSelectedItem(null);
                    loadDashboard();
                }}
                product={selectedItem}
                loading={modalLoading}
            />

            <ProductCategoryModal
                visible={categoryModalVisible}
                onClose={() => {
                    setCategoryModalVisible(false);
                    setSelectedItem(null);
                }}
                onSave={() => {
                    setCategoryModalVisible(false);
                    setSelectedItem(null);
                    loadDashboard();
                }}
                category={selectedItem}
                loading={modalLoading}
            />

            <SupplierModal
                visible={supplierModalVisible}
                onClose={() => {
                    setSupplierModalVisible(false);
                    setSelectedItem(null);
                }}
                onSave={() => {
                    setSupplierModalVisible(false);
                    setSelectedItem(null);
                    loadDashboard();
                }}
                supplier={selectedItem}
                loading={modalLoading}
            />
        </CContainer>
    );
};

export default InventoryDashboard;
