import { cilArrowTop } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { BestSellingProduct } from "../../../../types/basic/product/best-selling-product";
import formatCurrency from "../../../../utils/format-currency";

interface BestSellingProductsProps {
    data: BestSellingProduct[];
}

export default function BestSellingProducts({ data }: BestSellingProductsProps) {
    const { t } = useTranslation();
    
    return (<CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilArrowTop} className="me-2 text-success" />
                    <strong>{t('products.bestSellingProducts')}</strong>
                </CCardHeader>
                <CCardBody>
                    {data.length === 0 ? (
                        <p className="text-muted text-center">{t('common.noData')}</p>
                    ) : (
                        <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>#</CTableHeaderCell>
                                    <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                    <CTableHeaderCell>{t('products.category')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('products.quantitySold')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('products.revenue')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('products.orders')}</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {data.map((product, index) => (
                                    <CTableRow key={product.productId}>
                                        <CTableDataCell>{index + 1}</CTableDataCell>
                                        <CTableDataCell>
                                            <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                <strong>{product.productName}</strong>
                                            </Link>
                                        </CTableDataCell>
                                        <CTableDataCell>{product.categoryName}</CTableDataCell>
                                        <CTableDataCell className="text-center">{product.totalQuantitySold}</CTableDataCell>
                                        <CTableDataCell className="text-end">
                                            <strong>{formatCurrency(product.totalRevenue)}</strong>
                                        </CTableDataCell>
                                        <CTableDataCell className="text-center">{product.orderCount}</CTableDataCell>
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
