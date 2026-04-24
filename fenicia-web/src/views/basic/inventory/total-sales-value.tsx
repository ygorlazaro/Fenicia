import { cilTruck } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CWidgetStatsA } from "@coreui/react";
import { CChartLine } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { t } from "i18next";
import formatCurrency from "../../../utils/format-currency";

interface TotalSalesValueProps {
    totalSalesValue: number;
    profitPotential?: number;
}

export default function TotalSalesValue({ totalSalesValue, profitPotential }: TotalSalesValueProps) {
    const calculateProfitMargin = () => {
        if (totalSalesValue === 0) return 0
        return ((profitPotential / totalSalesValue) * 100).toFixed(1)
    }

    return (
        <CWidgetStatsA
            color="success"
            value={
                <>
                    {formatCurrency(totalSalesValue)}
                    <span className="fs-6 fw-normal d-block mt-1">
                        +{calculateProfitMargin()}% {t('inventory.margin')}
                    </span>
                </>
            }
            title={t('inventory.totalSalesValue')}
            action={
                <div className="mt-2">
                    <CIcon icon={cilTruck} size="xl" className="text-white-50" />
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
                                label: 'Sales',
                                backgroundColor: 'transparent',
                                borderColor: 'rgba(255,255,255,.55)',
                                pointBackgroundColor: getStyle('--cui-success'),
                                data: [1, 18, 9, 17, 34, 22, totalSalesValue ?? 11],
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
                        elements: { line: { borderWidth: 1 }, point: { radius: 4 } },
                    }}
                />
            }
        />
    );
}
