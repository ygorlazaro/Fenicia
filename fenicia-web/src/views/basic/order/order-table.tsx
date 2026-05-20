import { cilTrash } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CButton, CSpinner, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { default as Pagination, default as pagination } from "../../../components/fenicia/pagination";
import { GetAllOrderResponse } from "../../../types/basic/product-category/add-product-category-command";
import formatCurrency from "../../../utils/format-currency";
import formatDate from "../../../utils/format-date";

interface OrderTableProps {
    orders: GetAllOrderResponse[];
    loading: boolean;
    handlePageChange: (page: number) => void;
    handlePerPageChange: (perPage: number) => void;
    handleOpenDelete: (GetAllOrderResponse: GetAllOrderResponse) => void;
}

export default function OrderTable({ orders, loading, handlePageChange, handlePerPageChange, handleOpenDelete }: OrderTableProps) {
    const { t } = useTranslation();

    const getStatusBadgeColor = (status: string | null) => {
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

    if (loading) {
        return (
            <div className="text-center py-4">
                <CSpinner color="primary" />
                <p className="mt-2">{t("common.loading")}</p>
            </div>
        );
    }

    if (!loading && orders.length === 0) {
        return (
            <div className="text-center py-4">
                <p className="text-muted">{t("common.noData")}</p>
            </div>
        );
    }
    return (
        <>
            <CTable hover responsive>
                <CTableHead>
                    <CTableRow>
                        <CTableHeaderCell>{t("orders.id")}</CTableHeaderCell>
                        <CTableHeaderCell>{t("orders.customer")}</CTableHeaderCell>
                        <CTableHeaderCell>{t("orders.total")}</CTableHeaderCell>
                        <CTableHeaderCell>{t("orders.date")}</CTableHeaderCell>
                        <CTableHeaderCell>{t("orders.status")}</CTableHeaderCell>
                        <CTableHeaderCell>{t("orders.items")}</CTableHeaderCell>
                        <CTableHeaderCell className="text-end">{t("common.actions")}</CTableHeaderCell>
                    </CTableRow>
                </CTableHead>
                <CTableBody>
                    {orders.map((order) => (
                        <CTableRow key={order.id}>
                            <CTableDataCell>
                                <Link to={`/basic/order/${order.id}`} className="text-decoration-none font-monospace">
                                    {order.id.substring(0, 8)}...
                                </Link>
                            </CTableDataCell>
                            <CTableDataCell>
                                <Link to={`/basic/order/${order.id}`} className="text-decoration-none">
                                    {order.customerName}
                                </Link>
                            </CTableDataCell>
                            <CTableDataCell>{formatCurrency(order.totalAmount)}</CTableDataCell>
                            <CTableDataCell>{formatDate(order.saleDate)}</CTableDataCell>
                            <CTableDataCell>
                                <span className={`badge bg-${getStatusBadgeColor(order.status)}`}>{t(`orders.statusValues.${order.status?.toLowerCase()}`) || order.status}</span>
                            </CTableDataCell>
                            <CTableDataCell>{order.totalItems}</CTableDataCell>
                            <CTableDataCell className="text-end">
                                <CButton color="danger" size="sm" onClick={() => handleOpenDelete(order)}>
                                    <CIcon icon={cilTrash} />
                                </CButton>
                            </CTableDataCell>
                        </CTableRow>
                    ))}
                </CTableBody>
            </CTable>

            <Pagination pagination={pagination} onPageChange={handlePageChange} onPerPageChange={handlePerPageChange} />
        </>
    );
}
