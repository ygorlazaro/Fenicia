import CIcon from "@coreui/icons-react";
import { cilBan } from "@coreui/icons/dist/esm/free/cil-ban";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { ZeroMovementProduct } from "../../../../types/basic/inventory/zero-movement-product";
import formatCurrency from "../../../../utils/format-currency";

interface ZeroMovementProductsProps {
    data: ZeroMovementProduct[];
}

export default function ZeroMovementProducts({ data }: ZeroMovementProductsProps) {
    const { t } = useTranslation();
    return <CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilBan} className="me-2 text-danger" />
                    <strong>{t('inventory.zeroMovementProducts')}</strong>
                </CCardHeader>
                <CCardBody>
                    {data.length === 0 ? <p className="text-muted text-center">{t('inventory.allProductsMoving')}</p> : <CTable hover responsive>
                        <CTableHead>
                            <CTableRow>
                                <CTableHeaderCell>{t('inventory.product')}</CTableHeaderCell>
                                <CTableHeaderCell>{t('inventory.category')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('inventory.stock')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('inventory.stockValue')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('inventory.daysNoMovement')}</CTableHeaderCell>
                            </CTableRow>
                        </CTableHead>
                        <CTableBody>
                            {data.map(product => <CTableRow key={product.productId}>
                                <CTableDataCell>
                                    <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                        <strong>{product.productName}</strong>
                                    </Link>
                                </CTableDataCell>
                                <CTableDataCell>{product.categoryName}</CTableDataCell>
                                <CTableDataCell className="text-center">
                                    <CBadge color="warning">{product.currentStock}</CBadge>
                                </CTableDataCell>
                                <CTableDataCell className="text-end">
                                    <span className="text-danger">{formatCurrency(product.stockValue)}</span>
                                </CTableDataCell>
                                <CTableDataCell className="text-center">
                                    <CBadge color={product.daysWithoutMovement >= 180 ? 'danger' : product.daysWithoutMovement >= 120 ? 'warning' : 'info'}>
                                        {product.daysWithoutMovement} {t('inventory.days')}
                                    </CBadge>
                                </CTableDataCell>
                            </CTableRow>)}
                        </CTableBody>
                    </CTable>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
