import { cilChart } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { SupplierStockMovement } from "../../../../types/basic/supplier/supplier-stock-movement";
import formatCurrency from "../../../../utils/format-currency";

interface RecentStockMovementProps {
    data: SupplierStockMovement[];
}
export function RecentStockMovement({ data }: RecentStockMovementProps) {
    return <CRow>
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilChart} className="me-2" />
                    <strong>{t('suppliers.recentMovements')}</strong>
                </CCardHeader>
                <CCardBody>
                    {data.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <CTable hover responsive>
                        <CTableHead>
                            <CTableRow>
                                <CTableHeaderCell>{t('suppliers.date')}</CTableHeaderCell>
                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('suppliers.type')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('products.quantity')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('products.price')}</CTableHeaderCell>
                            </CTableRow>
                        </CTableHead>
                        <CTableBody>
                            {data.map(movement => <CTableRow key={movement.movementId}>
                                <CTableDataCell>{new Date(movement.date).toLocaleDateString()}</CTableDataCell>
                                <CTableDataCell>
                                    <Link to={`/basic/products?id=${movement.productId}`} className="text-decoration-none">
                                        {movement.productName}
                                    </Link>
                                </CTableDataCell>
                                <CTableDataCell className="text-center">
                                    <CBadge color={movement.movementType === 'In' ? 'success' : 'danger'}>
                                        {t(`suppliers.${movement.movementType.toLowerCase()}`)}
                                    </CBadge>
                                </CTableDataCell>
                                <CTableDataCell className="text-end">{movement.quantity}</CTableDataCell>
                                <CTableDataCell className="text-end">{formatCurrency(movement.price)}</CTableDataCell>
                            </CTableRow>)}
                        </CTableBody>
                    </CTable>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
