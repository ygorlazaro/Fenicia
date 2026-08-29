import { cilLayers } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CListGroup, CListGroupItem } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { CategoryBreakdown } from "../../../types/basic/inventory/category-breakdown";
import formatCurrency from "../../../utils/format-currency";
import { formatNumber } from "../../../utils/format-number";

interface BreakdownByCategoryProps {
    data: CategoryBreakdown[];
}

export default function BreakdownByCategory({ data }: BreakdownByCategoryProps) {
    return (
        <CCard className="mb-4">
            <CCardHeader className="d-flex align-items-center">
                <CIcon icon={cilLayers} className="me-2" size="lg" />
                <strong>{t("inventory.breakdownByCategory")}</strong>
            </CCardHeader>
            <CCardBody>
                {data.length === 0 ? (
                    <p className="text-muted text-center">{t("common.noData")}</p>
                ) : (
                    <CListGroup flush>
                        {data.slice(0, 5).map((category) => (
                            <CListGroupItem key={category.categoryId} className="d-flex justify-content-between align-items-center">
                                <div>
                                    <Link to={`/basic/product-categories?id=${category.categoryId}`} className="text-decoration-none">
                                        <div className="fw-semibold">{category.categoryName}</div>
                                    </Link>
                                    <small className="text-body-secondary">
                                        {formatNumber(category.totalQuantity)} {t("inventory.items")}
                                    </small>
                                </div>
                                <div className="text-end">
                                    <div className="text-success fw-semibold">{formatCurrency(category.totalSalesValue)}</div>
                                    <small className="text-body-secondary">
                                        {formatCurrency(category.totalCostValue)} {t("inventory.cost")}
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
