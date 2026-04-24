import { cilHistory } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { StockMovementHistory } from "../../../types/basic/stock-movement/stock-movement-history";
import formatCurrency from "../../../utils/format-currency";
import formatDate from "../../../utils/format-date";
import { formatNumber } from "../../../utils/format-number";

interface StockMovementHistoryProps {
    history: StockMovementHistory[] | null;
    openProductModal: (productId: string, event: React.MouseEvent<HTMLAnchorElement>) => void;
    openOrderModal: (orderId: string, event: React.MouseEvent<HTMLAnchorElement>) => void;
}

export default function StockMovementHistoryTable({ history = [], openProductModal, openOrderModal }: StockMovementHistoryProps) {
    const {t} = useTranslation();    
    const getTypeBadgeColor = (type: string) => {
        return type === 'In' ? 'success' : 'danger';
    };

    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilHistory} className="me-2 text-primary" size="lg" />
                        <strong>{t('stockMovement.history')}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {!history || history.length === 0 ? (
                            <p className="text-muted text-center">{t('common.noData')}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t('stockMovement.date')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('stockMovement.product')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t('stockMovement.type')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t('stockMovement.quantity')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t('stockMovement.price')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('stockMovement.order')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('stockMovement.reason')}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {history?.slice(0, 20).map((movement) => (
                                        <CTableRow key={movement.id}>
                                            <CTableDataCell>{formatDate(movement.date)}</CTableDataCell>
                                            <CTableDataCell>
                                                <a href={`/basic/products?id=${movement.productId}`} onClick={(e) => openProductModal(movement.productId, e)} className="text-decoration-none">
                                                    <div className="fw-semibold">{movement.productName}</div>
                                                </a>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <span className={`badge bg-${getTypeBadgeColor(movement.type)}`}>
                                                    {t(`stockMovement.${movement.type.toLowerCase()}`)}
                                                </span>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                {formatNumber(movement.quantity)}
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                {formatCurrency(movement.price)}
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                {movement.orderId ? (
                                                    <a href={`/basic/order/${movement.orderId}`} onClick={(e) => openOrderModal(movement.orderId, e)} className="text-primary">
                                                        {movement.orderId.substring(0, 8)}...
                                                    </a>
                                                ) : (
                                                    '-'
                                                )}
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                {movement.orderId ? (
                                                    <a href={`/basic/order/${movement.orderId}`} onClick={(e) => openOrderModal(movement.orderId, e)} className="text-decoration-none">
                                                        {movement.reason || '-'}
                                                    </a>
                                                ) : (
                                                    movement.reason || '-'
                                                )}
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>
                        )}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    )
}
