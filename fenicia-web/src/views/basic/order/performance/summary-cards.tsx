import { CCol, CRow, CWidgetStatsA } from "@coreui/react";
import { t } from "i18next";
import { AverageOrderValue } from "../../../../types/basic/order/average-order-value";
import formatCurrency from "../../../../utils/format-currency";

interface SummaryCardsProps {
    averageOrderValue: AverageOrderValue;
    cancelledOrderLength: number;
}

export default function SummaryCards({ averageOrderValue, cancelledOrderLength }: SummaryCardsProps) {
    return (<CRow className="mb-4" xs={{ gutter: 4 }}>
        <CCol sm={6} xl={3}>
            <CWidgetStatsA
                color="primary"
                value={<>
                    {formatCurrency(averageOrderValue.averageValue)}
                </>}
                title={t('orders.averageOrderValue')} />
        </CCol>

        <CCol sm={6} xl={3}>
            <CWidgetStatsA
                color="success"
                value={<>
                    {averageOrderValue.totalOrders}
                </>}
                title={t('orders.totalOrders')} />
        </CCol>

        <CCol sm={6} xl={3}>
            <CWidgetStatsA
                color="info"
                value={<>
                    {formatCurrency(averageOrderValue.medianValue)}
                </>}
                title={t('orders.medianValue')} />
        </CCol>

        <CCol sm={6} xl={3}>
            <CWidgetStatsA
                color="warning"
                value={<>
                    {cancelledOrderLength}
                </>}
                title={t('orders.cancelledOrders')} />
        </CCol>
    </CRow>
    );
}
