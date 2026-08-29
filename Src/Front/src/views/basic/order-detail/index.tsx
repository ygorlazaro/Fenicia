import { cilArrowLeft, cilPrint } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CAlert, CButton, CContainer, CSpinner } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import { BasicOrderClient } from "../../../services/basic/basic-order-client";
import { OrderDetailResponse } from "../../../types/basic/order/order-detail-response";
import { GetOrderByIdResponse } from "../../../types/basic/product-category/add-product-category-command";
import OrderHeader from "./order-header";
import OrderInformation from "./order-information";
import OrderItems from "./order-items";

const orderClient = new BasicOrderClient();

const OrderDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { t } = useTranslation();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [order, setOrder] = useState<GetOrderByIdResponse | null>(null);
    const [details, setDetails] = useState<OrderDetailResponse[]>([]);

    useEffect(() => {
        loadOrderDetails();
    }, [id]);

    const loadOrderDetails = async () => {
        if (!id) return;

        try {
            setLoading(true);
            setError(null);

            const [orderData, detailsData] = await Promise.all([orderClient.getById(id), orderClient.getDetails(id)]);

            setOrder(orderData);
            setDetails(detailsData);
        } catch (err) {
            setError(t("orders.loadError"));
            console.error("Failed to load order details:", err);
        } finally {
            setLoading(false);
        }
    };

    const handlePrint = () => {
        window.print();
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

    if (error || !order) {
        return (
            <CContainer className="py-4">
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error || t("common.noData")}
                </CAlert>
                <CButton color="primary" onClick={() => navigate("/basic/orders")}>
                    <CIcon icon={cilArrowLeft} className="me-2" />
                    {t("common.back")}
                </CButton>
            </CContainer>
        );
    }

    return (
        <CContainer className="py-4">
            {/* Print Styles */}
            <style>{`
                @media print {
                    .no-print {
                        display: none !important;
                    }
                    .card {
                        border: none !important;
                        box-shadow: none !important;
                    }
                    body {
                        background: white !important;
                    }
                }
            `}</style>

            {/* Header Actions */}
            <div className="d-flex justify-content-between align-items-center mb-4 no-print">
                <CButton color="primary" onClick={() => navigate("/basic/orders")}>
                    <CIcon icon={cilArrowLeft} className="me-2" />
                    {t("common.back")}
                </CButton>
                <CButton color="secondary" onClick={handlePrint}>
                    <CIcon icon={cilPrint} className="me-2" />
                    {t("common.print")}
                </CButton>
            </div>

            <OrderHeader order={order} />

            <OrderItems details={details} totalAmount={order.totalAmount} />

            <OrderInformation order={order} />
        </CContainer>
    );
};

export default OrderDetailPage;
