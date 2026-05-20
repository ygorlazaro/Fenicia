import { cilCalendar, cilCart, cilDollar, cilUser } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { GetOrderByIdResponse } from "../../../types/basic/order/get-order-by-id-response";
import formatCurrency from "../../../utils/format-currency";
import formatDate from "../../../utils/format-date";

interface OrderHeaderProps {
    order: GetOrderByIdResponse;
}

export default function OrderHeader({ order }: OrderHeaderProps) {
    const { t } = useTranslation();

    const getStatusBadgeColor = (status: string): string => {
        switch (status?.toLowerCase()) {
            case "pending":
                return "warning";
            case "approved":
                return "success";
            case "cancelled":
                return "danger";
            default:
                return "secondary";
        }
    };
    return (
        <CCard className="mb-4">
            <CCardHeader className="d-flex align-items-center">
                <CIcon icon={cilCart} className="me-2" size="lg" />
                <strong>
                    {t("orders.orderDetails")} #{order.id.substring(0, 8)}
                </strong>
            </CCardHeader>
            <CCardBody>
                <CRow xs={{ gutter: 3 }}>
                    <CCol md={4}>
                        <div className="text-muted small">{t("orders.customer")}</div>
                        <Link to={`/basic/customers?id=${order.customerId}`} className="text-decoration-none">
                            <div className="d-flex align-items-center">
                                <CIcon icon={cilUser} className="me-2 text-primary" />
                                <strong>{order.customerName}</strong>
                            </div>
                        </Link>
                    </CCol>
                    <CCol md={4}>
                        <div className="text-muted small">{t("orders.date")}</div>
                        <div className="d-flex align-items-center">
                            <CIcon icon={cilCalendar} className="me-2 text-primary" />
                            <strong>{formatDate(order.saleDate)}</strong>
                        </div>
                    </CCol>
                    <CCol md={4}>
                        <div className="text-muted small">{t("orders.statusLabel")}</div>
                        <CBadge color={getStatusBadgeColor(order.status)} size="sm">
                            {t(`orders.statusValues.${order.status.toLowerCase()}`)}
                        </CBadge>
                    </CCol>
                    <CCol md={12} className="border-top pt-3 mt-2">
                        <div className="d-flex justify-content-between align-items-center">
                            <div className="d-flex align-items-center">
                                <CIcon icon={cilDollar} className="me-2 text-success" size="lg" />
                                <span className="text-muted">{t("orders.totalAmount")}</span>
                            </div>
                            <div className="text-success">
                                <strong className="fs-3">{formatCurrency(order.totalAmount)}</strong>
                            </div>
                        </div>
                    </CCol>
                </CRow>
            </CCardBody>
        </CCard>
    );
}
