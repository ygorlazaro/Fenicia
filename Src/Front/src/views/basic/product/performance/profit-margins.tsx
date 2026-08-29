import { cilChart, cilDollar } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { CChartPie } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { ProfitMargin } from "../../../../types/basic/product/profit-margin";
import formatCurrency from "../../../../utils/format-currency";

interface ProfitMarginsProps {
    data: ProfitMargin[];
}

export default function ProfitMargins({ data }: ProfitMarginsProps) {
    const { t } = useTranslation();

    const getMarginDistributionData = () => {
        if (data.length === 0) return null;
        const distribution = {
            Excellent: 0,
            Good: 0,
            Average: 0,
            Low: 0,
            "Very Low": 0
        };
        data.forEach((p) => {
            if (distribution[p.marginClassification] !== undefined) {
                distribution[p.marginClassification]++;
            }
        });
        return {
            labels: Object.keys(distribution),
            datasets: [
                {
                    data: Object.values(distribution),
                    backgroundColor: [getStyle("--cui-success"), getStyle("--cui-info"), getStyle("--cui-warning"), getStyle("--cui-orange"), getStyle("--cui-danger")]
                }
            ]
        };
    };

    const getMarginBadgeColor = (classification: string) => {
        switch (classification?.toLowerCase()) {
            case "excellent":
                return "success";
            case "good":
                return "info";
            case "average":
                return "warning";
            case "low":
                return "orange";
            default:
                return "danger";
        }
    };

    return (
        <CRow className="mb-4" xs={{ gutter: 4 }}>
            <CCol md={6}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilChart} className="me-2" />
                        <strong>{t("products.marginDistribution")}</strong>
                    </CCardHeader>
                    <CCardBody>{data.length === 0 ? <p className="text-muted text-center">{t("common.noData")}</p> : <CChartPie data={getMarginDistributionData()} options={{ responsive: true, maintainAspectRatio: true }} />}</CCardBody>
                </CCard>
            </CCol>
            <CCol md={6}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilDollar} className="me-2" />
                        <strong>{t("products.profitMargins")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {data.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("products.name")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("products.cost")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("products.price")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("products.margin")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {data.slice(0, 10).map((product) => (
                                        <CTableRow key={product.productId}>
                                            <CTableDataCell>
                                                <Link to={`/basic/products?id=${product.productId}`} className="text-decoration-none">
                                                    {product.productName}
                                                </Link>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">{formatCurrency(product.costPrice)}</CTableDataCell>
                                            <CTableDataCell className="text-end">{formatCurrency(product.salesPrice)}</CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <CBadge color={getMarginBadgeColor(product.marginClassification)}>
                                                    {product.profitMargin.toFixed(1)}% ({product.marginClassification})
                                                </CBadge>
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
