import { CSpinner } from "@coreui/react";
import { t } from "i18next";
import { InventoryHealth } from "../../../../types/basic/inventory/inventory-health";
import OverstockAlert from "./overstock-alert";
import { StockValueByCategory } from './stock-value-by-category';
import { SummaryCards } from './summary-cards';
import ZeroMovementProducts from './zero-movement-products';

interface InventoryHealthData {
    healthLoading: boolean
    health: InventoryHealth | null
}

export default function RenderHealthTab({ healthLoading, health }: InventoryHealthData) {
    if (healthLoading) {
        return (
            <div className="text-center py-5">
                <CSpinner color="primary" />
                <p className="mt-3">{t('common.loading')}</p>
            </div>
        )
    }

    if (!health) {
        return (
            <div className="text-center py-5">
                <p className="text-muted">{t('common.noData')}</p>
            </div>
        )
    }

    return (
        <>
            <SummaryCards data={health.summary} totalOverstockValue={health.overstockAlert.totalOverstockValue} totalProducts={health.overstockAlert.totalOverstockProducts} zeroMovementProducts={health.zeroMovementProducts} />

            <OverstockAlert data={health.overstockAlert} />

            <ZeroMovementProducts data={health.zeroMovementProducts} />

            <StockValueByCategory data={health.stockValueByCategory} />
        </>
    )
}
