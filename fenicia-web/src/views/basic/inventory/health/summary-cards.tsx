import { CCol, CRow, CWidgetStatsB } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { InventoryHealthSummary } from "../../../../types/basic/inventory/inventory-health-summary";
import { ZeroMovementProduct } from "../../../../types/basic/inventory/zero-movement-product";
import formatCurrency from "../../../../utils/format-currency";
import formatPercentage from "../../../../utils/format-percentage";

interface SummaryCardsProps {
    data: InventoryHealthSummary;
    totalOverstockValue: number;
    zeroMovementProducts: ZeroMovementProduct[];
    totalProducts: number;
}

export function SummaryCards({ data, totalOverstockValue, zeroMovementProducts, totalProducts }: SummaryCardsProps) {
    const { t } = useTranslation();

    return <CRow className="mb-4" xs={{
        gutter: 4
    }}>
        <CCol sm={6} xl={3}>
            <CWidgetStatsB color="success" title={t('inventory.healthyProducts')} value={<>
                {data.healthyProducts}
                <span className="fs-6 fw-normal d-block mt-1">
                    {formatPercentage(100 - data.overstockPercentage - data.zeroMovementPercentage)} {t('inventory.ofTotal')}
                </span>
            </>} />
        </CCol>
        <CCol sm={6} xl={3}>
            <CWidgetStatsB color="warning" title={t('inventory.overstockProducts')} value={<>
                {data.overstockProducts}
                <span className="fs-6 fw-normal d-block mt-1">
                    {formatCurrency(totalOverstockValue)} {t('inventory.tiedUp')}
                </span>
            </>} />
        </CCol>
        <CCol sm={6} xl={3}>
            <CWidgetStatsB color="danger" title={t('inventory.zeroMovementProducts')} value={<>
                {zeroMovementProducts.length}
                <span className="fs-6 fw-normal d-block mt-1">
                    {formatCurrency(zeroMovementProducts.reduce((sum, p) => sum + p.stockValue, 0))} {t('inventory.atRisk')}
                </span>
            </>} />
        </CCol>
        <CCol sm={6} xl={3}>
            <CWidgetStatsB color="info" title={t('inventory.totalStockValue')} value={<>
                {formatCurrency(data.totalStockValue)}
                <span className="fs-6 fw-normal d-block mt-1">
                    {totalProducts} {t('inventory.products')}
                </span>
            </>} />
        </CCol>
    </CRow>;
}
