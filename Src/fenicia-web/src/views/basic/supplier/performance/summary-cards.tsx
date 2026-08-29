import { CCol, CRow, CWidgetStatsA } from "@coreui/react";
import { t } from "i18next";
import { SupplierSummary } from "../../../../types/basic/supplier/supplier-summary";
import formatCurrency from "../../../../utils/format-currency";

interface SummaryCardsProps {
    data: SupplierSummary;
}

export default function SummaryCards({ data }: SummaryCardsProps) {
    return (
        <CRow
            className="mb-4"
            xs={{
                gutter: 4
            }}
        >
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="primary"
                    value={
                        <>
                            {data.totalSuppliers}
                            <span className="fs-6 fw-normal d-block mt-1">{t("suppliers.suppliers")}</span>
                        </>
                    }
                    title={t("suppliers.totalSuppliers")}
                />
            </CCol>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="success"
                    value={
                        <>
                            {data.totalProducts}
                            <span className="fs-6 fw-normal d-block mt-1">{t("suppliers.products")}</span>
                        </>
                    }
                    title={t("suppliers.totalProducts")}
                />
            </CCol>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="info"
                    value={
                        <>
                            {formatCurrency(data.totalStockValue)}
                            <span className="fs-6 fw-normal d-block mt-1">{t("suppliers.stockValue")}</span>
                        </>
                    }
                    title={t("suppliers.totalStockValue")}
                />
            </CCol>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="warning"
                    value={
                        <>
                            {data.averageProductsPerSupplier.toFixed(1)}
                            <span className="fs-6 fw-normal d-block mt-1">{t("suppliers.avgPerSupplier")}</span>
                        </>
                    }
                    title={t("suppliers.averageProductsPerSupplier")}
                />
            </CCol>
        </CRow>
    );
}
