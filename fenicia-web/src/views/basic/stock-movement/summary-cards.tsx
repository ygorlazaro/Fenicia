import { cilArrowBottom, cilArrowTop, cilHistory, cilSpeedometer } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCol, CRow, CWidgetStatsA } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { StockMovementDashboard } from "../../../types/basic/stock-movement/stock-movement-dashboard";
import { formatNumber } from "../../../utils/format-number";

interface SummaryCardsProps {
    dashboard: StockMovementDashboard | null;
}

export default function SummaryCards({ dashboard }: SummaryCardsProps) {
    const { t } = useTranslation();

    const getTotalInQuantity = () => {
        if (!dashboard || !dashboard.monthlyInOut) return 0;
        return dashboard.monthlyInOut.reduce((sum, m) => sum + m.totalIn, 0);
    };

    const getTotalOutQuantity = () => {
        if (!dashboard || !dashboard.monthlyInOut) return 0;
        return dashboard.monthlyInOut.reduce((sum, m) => sum + m.totalOut, 0);
    };

    const getTotalMovements = () => {
        if (!dashboard || !dashboard.history) return 0;
        return dashboard.history.length;
    };

    const getAverageTurnover = () => {
        if (!dashboard || !dashboard.turnoverRates || dashboard.turnoverRates.length === 0) return 0;
        const sum = dashboard.turnoverRates.reduce((acc, t) => acc + t.turnoverRate, 0);
        return (sum / dashboard.turnoverRates.length).toFixed(2);
    };


    return (
        <CRow className="mb-4" xs={{ gutter: 4 }}>
            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="success"
                    value={
                        <>
                            {formatNumber(getTotalInQuantity())}
                            <span className="fs-6 fw-normal d-block mt-1">
                                {t('stockMovement.unitsIn')}
                            </span>
                        </>
                    }
                    title={t('stockMovement.totalIn')}
                    action={
                        <div className="mt-2">
                            <CIcon icon={cilArrowTop} size="xl" className="text-white-50" />
                        </div>
                    }
                />
            </CCol>

            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="danger"
                    value={
                        <>
                            {formatNumber(getTotalOutQuantity())}
                            <span className="fs-6 fw-normal d-block mt-1">
                                {t('stockMovement.unitsOut')}
                            </span>
                        </>
                    }
                    title={t('stockMovement.totalOut')}
                    action={
                        <div className="mt-2">
                            <CIcon icon={cilArrowBottom} size="xl" className="text-white-50" />
                        </div>
                    }
                />
            </CCol>

            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="primary"
                    value={
                        <>
                            {getTotalMovements()}
                            <span className="fs-6 fw-normal d-block mt-1">
                                {t('stockMovement.movements')}
                            </span>
                        </>
                    }
                    title={t('stockMovement.totalMovements')}
                    action={
                        <div className="mt-2">
                            <CIcon icon={cilHistory} size="xl" className="text-white-50" />
                        </div>
                    }
                />
            </CCol>

            <CCol sm={6} xl={3}>
                <CWidgetStatsA
                    color="info"
                    value={
                        <>
                            {getAverageTurnover()}
                            <span className="fs-6 fw-normal d-block mt-1">
                                {t('stockMovement.avgTurnover')}
                            </span>
                        </>
                    }
                    title={t('stockMovement.averageTurnover')}
                    action={
                        <div className="mt-2">
                            <CIcon icon={cilSpeedometer} size="xl" className="text-white-50" />
                        </div>
                    }
                />
            </CCol>
        </CRow>
    )
} 
