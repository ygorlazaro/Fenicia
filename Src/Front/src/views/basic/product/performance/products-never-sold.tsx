import { cilBan } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { NeverSoldProduct } from "../../../../types/basic/product/never-sold-product";
import formatCurrency from "../../../../utils/format-currency";

interface ProductNeverSoldProps {
    data: NeverSoldProduct[];
}

export default function ProductNeverSold({ data }: ProductNeverSoldProps) {
    return (
        <CRow>
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilBan} className="me-2 text-danger" />
                        <strong>{t("products.neverSoldProducts")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {data.length === 0 ? (
                            <p className="text-muted text-center">{t("products.allProductsSelling")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("products.name")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("products.category")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("products.supplier")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("products.stock")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("products.costValue")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {data.map((product) => (
                                        <CTableRow key={product.productId}>
                                            <CTableDataCell>
                                                <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                    <strong>{product.productName}</strong>
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell>{product.categoryName}</CTableDataCell>
                                            <CTableDataCell>{product.supplierName || "-"}</CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <CBadge color="danger">{product.currentStock}</CBadge>
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
