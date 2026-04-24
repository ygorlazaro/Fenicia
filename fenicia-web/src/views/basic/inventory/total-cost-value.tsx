import { cilDollar } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CWidgetStatsA } from "@coreui/react";
import { CChartLine } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { t } from "i18next";
import formatCurrency from "../../../utils/format-currency";
import { formatNumber } from "../../../utils/format-number";

interface TotalCostValueProps {
    totalCostValue: number;
    totalQuantity: number;
}

export default function TotalCostValue({ totalCostValue, totalQuantity }: TotalCostValueProps) {
    return (
        <CWidgetStatsA
            color="primary"
            value={
                <>
                    {formatCurrency(totalCostValue)}
                    <span className="fs-6 fw-normal d-block mt-1">
                        {formatNumber(totalQuantity)} {t('inventory.items')}
                    </span>
                </>
            }
            title={t('inventory.totalCostValue')}
            action={
                <div className="mt-2">
                    <CIcon icon={cilDollar} size="xl" className="text-white-50" />
                </div>
            }
            chart={
                <CChartLine
                    className="mt-3 mx-3"
                    style={{ height: '70px' }}
                    data={{
                        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                        datasets: [
                            {
                                label: 'Cost',
                                backgroundColor: 'transparent',
                                borderColor: 'rgba(255,255,255,.55)',
                                pointBackgroundColor: getStyle('--cui-primary'),
                                data: [65, 59, 84, 84, 51, 55, totalCostValue ?? 40],
                            },
                        ],
                    }}
                    options={{
                        plugins: { legend: { display: false } },
                        maintainAspectRatio: false,
                        scales: {
                            x: { border: { display: false }, grid: { display: false }, ticks: { display: false } },
                            y: { display: false, grid: { display: false }, ticks: { display: false } },
                        },
                        elements: { line: { borderWidth: 1, tension: 0.4 }, point: { radius: 4 } },
                    }}
                />
            }
        />
    )
}
