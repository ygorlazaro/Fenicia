import CIcon from "@coreui/icons-react";
import { cilTruck } from "@coreui/icons/dist/esm/free/cil-truck";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { SupplierProductCount } from "../../../../types/basic/supplier/supplier-product-count";
import formatCurrency from "../../../../utils/format-currency";

interface ProductsPerSupplierProps {
    data: SupplierProductCount[];
}

export function ProductsPerSupplier({ data }: ProductsPerSupplierProps) {
    const { t } = useTranslation();

    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilTruck} className="me-2" />
                        <strong>{t("suppliers.productsPerSupplier")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {data.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("suppliers.name")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("suppliers.products")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("suppliers.stockValue")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("suppliers.revenue")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {data.map((supplier, index) => (
                                        <CTableRow key={supplier.supplierId}>
                                            <CTableDataCell>
                                                <Link to={`/basic/suppliers?id=${supplier.supplierId}`} className="text-decoration-none">
                                                    <strong>{supplier.supplierName}</strong>
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">{supplier.productCount}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatCurrency(supplier.totalStockValue)}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatCurrency(supplier.totalRevenue)}</CTableDataCell>
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
