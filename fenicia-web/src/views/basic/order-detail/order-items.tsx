import { CCard, CCardBody, CCardHeader, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { OrderDetailResponse } from "../../../types/basic/order/order-detail-response";
import formatCurrency from "../../../utils/format-currency";

interface OrderItemsProps {
    details: OrderDetailResponse[];
    totalAmount: number;
}

export default function OrderItems({ details, totalAmount }: OrderItemsProps) { 
    const { t } = useTranslation();

    return (<CCard className="mb-4">
        <CCardHeader>
            <strong>{t('orders.items')}</strong>
        </CCardHeader>
        <CCardBody>
            {details.length === 0 ? (
                <p className="text-muted text-center">{t('common.noData')}</p>
            ) : (
                <CTable hover responsive>
                    <CTableHead>
                        <CTableRow>
                            <CTableHeaderCell>#</CTableHeaderCell>
                            <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                            <CTableHeaderCell className="text-center">{t('orders.quantity')}</CTableHeaderCell>
                            <CTableHeaderCell className="text-end">{t('products.price')}</CTableHeaderCell>
                            <CTableHeaderCell className="text-end">{t('orders.subtotal')}</CTableHeaderCell>
                        </CTableRow>
                    </CTableHead>
                    <CTableBody>
                        {details.map((item, index) => (
                            <CTableRow key={item.productId}>
                                <CTableDataCell>{index + 1}</CTableDataCell>
                                <CTableDataCell>
                                    <Link to={`/basic/products?id=${item.productId}`} className="text-decoration-none">
                                        <strong>{item.productName}</strong>
                                    </Link>
                                </CTableDataCell>
                                <CTableDataCell className="text-center">
                                    {item.quantity}
                                </CTableDataCell>
                                <CTableDataCell className="text-end">
                                    {formatCurrency(item.price)}
                                </CTableDataCell>
                                <CTableDataCell className="text-end">
                                    <strong>{formatCurrency(item.subtotal)}</strong>
                                </CTableDataCell>
                            </CTableRow>
                        ))}
                    </CTableBody>
                    <CTableBody>
                        <CTableRow>
                            <CTableDataCell colSpan={4} className="text-end fw-bold">
                                {t('orders.total')}:
                            </CTableDataCell>
                            <CTableDataCell className="text-end fw-bold text-success">
                                {formatCurrency(totalAmount)}
                            </CTableDataCell>
                        </CTableRow>
                    </CTableBody>
                </CTable>
            )}
        </CCardBody>
    </CCard>)
}
