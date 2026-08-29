import { cilBan } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { OrderCancelledOrder } from "../../../../types/basic/order/order-cancelled-order";
import formatCurrency from "../../../../utils/format-currency";
import formatDate from "../../../../utils/format-date";

interface CancelledOrderProps {
    cancelledOrders: OrderCancelledOrder[];
}

export default function CancelledOrder({ cancelledOrders }: CancelledOrderProps) {
    return (
        <CRow>
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilBan} className="me-2 text-danger" />
                        <strong>{t("orders.cancelledOrdersReport")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {cancelledOrders.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("orders.date")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("orders.customer")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("orders.totalAmount")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("orders.items")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {cancelledOrders.map((order) => (
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
    );
}
