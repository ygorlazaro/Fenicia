import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { GetOrderByIdResponse } from "../../../types/basic/order/get-order-by-id-response";

interface OrderInformationProps {
    order: GetOrderByIdResponse;
}

export default function OrderInformation({ order }: OrderInformationProps) {
    const { t } = useTranslation();

    return (
        <CCard>
            <CCardHeader>
                <strong>{t("orders.information")}</strong>
            </CCardHeader>
            <CCardBody>
                <CRow xs={{ gutter: 3 }}>
                    <CCol md={6}>
                        <p className="mb-1 text-muted small">Order ID</p>
                        <Link to={`/basic/order/${order.id}`} className="font-monospace text-decoration-none">
                            {order.id}
                        </Link>
                    </CCol>
                    <CCol md={6}>
                        <p className="mb-1 text-muted small">{t("orders.customer")} ID</p>
                        <Link to={`/basic/customers?id=${order.customerId}`} className="font-monospace text-decoration-none">
                            {order.customerId}
                        </Link>
                    </CCol>
                    {order.employeeId && (
                        <CCol md={6}>
                            <p className="mb-1 text-muted small">{t("orders.employee")} ID</p>
                            <Link to={`/basic/employees?id=${order.employeeId}`} className="font-monospace text-decoration-none">
                                {order.employeeId}
                            </Link>
                        </CCol>
                    )}
                </CRow>
            </CCardBody>
        </CCard>
    );
}
