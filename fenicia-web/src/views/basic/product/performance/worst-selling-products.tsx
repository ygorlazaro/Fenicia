import { cilArrowBottom } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { WorstSellingProduct } from "../../../../types/basic/product/worst-selling-product";
import formatCurrency from "../../../../utils/format-currency";

interface WorstSellingProductsProps {
    data: WorstSellingProduct[];
}

export default function WorstSellingProducts({ data }: WorstSellingProductsProps) {
    const { t } = useTranslation();

    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilArrowBottom} className="me-2 text-warning" />
                        <strong>{t('products.worstSellingProducts')}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {data.length === 0 ? (
                            <p className="text-muted text-center">{t('common.noData')}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                        <CTableHeaderCell>{t('products.category')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t('products.sold')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t('products.stock')}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t('products.costValue')}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {data.map((product) => (
                                        <CTableRow key={product.productId}>
                                            <CTableDataCell>
                                                <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                    {product.productName}
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell>{product.categoryName}</CTableDataCell>
                                            <CTableDataCell className="text-center">{product.totalQuantitySold}</CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <CBadge color={product.currentStock > 50 ? 'warning' : 'info'}>
                                                    {product.currentStock}
                                                </CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <span className="text-danger">{formatCurrency(product.costValue)}</span>
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
    );
}
