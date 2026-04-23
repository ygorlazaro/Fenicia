import { cilDollar } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { SupplierCostComparison } from "../../../../types/basic/supplier/supplier-cost-comparison";
import formatCurrency from "../../../../utils/format-currency";

interface CostComparisonProps {
    data: SupplierCostComparison[];
}

export function CostComparison({ data }: CostComparisonProps) {
    const { t } = useTranslation();

    return <CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilDollar} className="me-2" />
                    <strong>{t('suppliers.costComparison')}</strong>
                </CCardHeader>
                <CCardBody>
                    {data.length === 0 ? <p className="text-muted text-center">{t('suppliers.noCostComparison')}</p> : <CTable hover responsive>
                        <CTableHead>
                            <CTableRow>
                                <CTableHeaderCell>{t('products.name')}</CTableHeaderCell>
                                <CTableHeaderCell>{t('suppliers.supplier')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('products.costPrice')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('products.salesPrice')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('suppliers.margin')}</CTableHeaderCell>
                            </CTableRow>
                        </CTableHead>
                        <CTableBody>
                            {data.map(product => product.suppliers.map((supplier, idx) => <CTableRow key={`${product.productName}-${supplier.supplierId}`}>
                                <CTableDataCell rowSpan={product.suppliers.length}>{product.productName}</CTableDataCell>
                                <CTableDataCell>
                                    <Link to={`/basic/suppliers?id=${supplier.supplierId}`} className="text-decoration-none">
                                        {supplier.supplierName}
                                    </Link>
                                </CTableDataCell>
                                <CTableDataCell className="text-end">{formatCurrency(supplier.costPrice)}</CTableDataCell>
                                <CTableDataCell className="text-end">{formatCurrency(supplier.salesPrice)}</CTableDataCell>
                                <CTableDataCell className="text-center">
                                    <CBadge color={supplier.profitMargin >= 30 ? 'success' : supplier.profitMargin >= 15 ? 'warning' : 'danger'}>
                                        {supplier.profitMargin.toFixed(1)}%
                                    </CBadge>
                                </CTableDataCell>
                            </CTableRow>))}
                        </CTableBody>
                    </CTable>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
