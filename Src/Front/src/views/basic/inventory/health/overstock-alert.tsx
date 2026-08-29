import { cilWarning } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { InventoryOverstockAlert } from "../../../../types/basic/inventory/inventory-overstock-alert";
import formatCurrency from "../../../../utils/format-currency";

interface OverstockAlertProps {
    data: InventoryOverstockAlert;
}

export default function OverstockAlert({ data }: OverstockAlertProps) {
    const { t } = useTranslation();
    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        <strong>{t("inventory.overstockAlert")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {data.products.length === 0 ? (
                            <p className="text-muted text-center">{t("inventory.noOverstock")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("inventory.product")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("inventory.category")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("inventory.currentQty")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("inventory.recommendedQty")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("inventory.excessValue")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {data.products.map((product) => (
                                        <CTableRow key={product.productId}>
                                            <CTableDataCell>
                                                <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                    <strong>{product.productName}</strong>
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell>{product.categoryName}</CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <CBadge color="danger">{product.currentQuantity}</CBadge>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">{product.recommendedQuantity.toFixed(0)}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <span className="text-danger">{formatCurrency(product.excessValue)}</span>
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
