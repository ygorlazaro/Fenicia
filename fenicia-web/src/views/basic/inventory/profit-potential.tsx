import { cilArrowBottom, cilArrowTop, cilDollar } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CWidgetStatsA } from "@coreui/react";
import { CChartBar } from "@coreui/react-chartjs";
import { t } from "i18next";
import formatCurrency from "../../../utils/format-currency";

interface ProfitPotentialProps {
    profitPotential?: number;
}

export default function ProfitPotential({ profitPotential }: ProfitPotentialProps) { 
    return (
        <CWidgetStatsA
            color="warning"
            value={
                <>
                    {formatCurrency(profitPotential ?? 0)}
                    <span className="fs-6 fw-normal d-block mt-1">
                        {profitPotential >= 0 ? (
                            <>
                                <CIcon icon={cilArrowTop} /> {t('inventory.profit')}
                            </>
                        ) : (
                            <>
                                <CIcon icon={cilArrowBottom} /> {t('inventory.loss')}
                            </>
                        )}
                    </span>
                </>
            }
            title={t('inventory.profitPotential')}
            action={
                <div className="mt-2">
                    <CIcon icon={cilDollar} size="xl" className="text-white-50" />
                </div>
            }
            chart={
                <CChartBar
                    className="mt-3 mx-3"
                    style={{ height: '70px' }}
                    data={{
                        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                        datasets: [
                            {
                                label: 'Profit',
                                backgroundColor: 'rgba(255,255,255,.2)',
                                borderColor: 'rgba(255,255,255,.55)',
                                data: [78, 81, 80, 45, 34, 12, 40, 85, 65, 23, 12, profitPotential ?? 82],
                                barPercentage: 0.6,
                            },
                        ],
                    }}
                    options={{
                        maintainAspectRatio: false,
                        plugins: { legend: { display: false } },
                        scales: {
                            x: { grid: { display: false, drawTicks: false }, ticks: { display: false } },
                            y: { border: { display: false }, grid: { display: false }, ticks: { display: false } },
                        },
                    }}
                />
            }
        />
    )
} 
