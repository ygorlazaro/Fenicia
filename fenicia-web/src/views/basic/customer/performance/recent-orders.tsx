import { cilClock } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { CustomerRecentOrder } from "../../../../types/basic/customer/customer-recent-order";
import formatCurrency from "../../../../utils/format-currency";
import formatDate from "../../../../utils/format-date";

interface RecentOrdersProps {
    recentOrders: CustomerRecentOrder[];
}

export function RecentOrders({ recentOrders }: RecentOrdersProps) {
    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilClock} className="me-2" />
                        <strong>{t("customers.recentOrders")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {recentOrders.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("customers.date")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("customers.customer")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("customers.totalAmount")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("customers.status")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("customers.items")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {recentOrders.map((order) => (
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
                                                <CBadge color={order.status === "Approved" ? "success" : order.status === "Pending" ? "warning" : "danger"}>{t(`orders.statusValues.${order.status.toLowerCase()}`)}</CBadge>
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
