import { cilTruck } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CListGroup, CListGroupItem } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { SupplierBreakdown } from "../../../types/basic/inventory/supplier-breakdown";
import formatCurrency from "../../../utils/format-currency";
import { formatNumber } from "../../../utils/format-number";

interface BreakdownBySupplierProps {
    data: SupplierBreakdown[];
}

export default function BreakdownBySupplier({ data }: BreakdownBySupplierProps) {
    return (
        <CCard className="mb-4">
            <CCardHeader className="d-flex align-items-center">
                <CIcon icon={cilTruck} className="me-2" size="lg" />
                <strong>{t("inventory.breakdownBySupplier")}</strong>
            </CCardHeader>
            <CCardBody>
                {data.length === 0 ? (
                    <p className="text-muted text-center">{t("common.noData")}</p>
                ) : (
                    <CListGroup flush>
                        {data.slice(0, 5).map((supplier, index) => (
                            <CListGroupItem key={supplier.supplierId} className="d-flex justify-content-between align-items-center">
                                <div>
                                    <Link to={`/basic/suppliers?id=${supplier.supplierId}`} className="text-decoration-none">
                                        <div className="fw-semibold">{supplier.supplierName}</div>
                                    </Link>
                                    <small className="text-body-secondary">
                                        {formatNumber(supplier.totalQuantity)} {t("inventory.items")}
                                    </small>
                                </div>
                                <div className="text-end">
                                    <div className="text-success fw-semibold">{formatCurrency(supplier.totalSalesValue)}</div>
                                    <small className="text-body-secondary">
                                        {formatCurrency(supplier.totalCostValue)} {t("inventory.cost")}
                                    </small>
                                </div>
                            </CListGroupItem>
                        ))}
                    </CListGroup>
                )}
            </CCardBody>
        </CCard>
    );
}
