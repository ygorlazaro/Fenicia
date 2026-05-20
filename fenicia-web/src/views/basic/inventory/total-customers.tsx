import { cilPeople } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CWidgetStatsA } from "@coreui/react";
import { CChartDoughnut } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { t } from "i18next";

interface TotalCostValueProps {
    totalCustomers: number;
    totalEmployees: number;
}

export default function TotalCostValue({ totalCustomers, totalEmployees }: TotalCostValueProps) {
    return (
        <CWidgetStatsA
            color="info"
            value={
                <>
                    {totalCustomers ?? 0}
                    <span className="fs-6 fw-normal d-block mt-1">
                        {totalEmployees ?? 0} {t("inventory.employees")}
                    </span>
                </>
            }
            title={t("inventory.customersAndEmployees")}
            action={
                <div className="mt-2">
                    <CIcon icon={cilPeople} size="xl" className="text-white-50" />
                </div>
            }
            chart={
                <CChartDoughnut
                    className="mx-3"
                    style={{ height: "70px" }}
                    data={{
                        labels: [t("inventory.customers"), t("inventory.employees")],
                        datasets: [
                            {
                                backgroundColor: [getStyle("--cui-info"), getStyle("--cui-warning")],
                                data: [totalCustomers ?? 70, totalEmployees ?? 30]
                            }
                        ]
                    }}
                    options={{
                        plugins: { legend: { display: false } },
                        maintainAspectRatio: false
                    }}
                />
            }
        />
    );
}
