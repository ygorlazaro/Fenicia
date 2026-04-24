import CIcon from '@coreui/icons-react';
import { CCol, CWidgetStatsA } from '@coreui/react';

interface KpiSummaryCardProps {
    value: string | number;
    label: string;
    detail: string;
    color: 'success' | 'danger' | 'warning' | 'info' | 'primary' | 'secondary';
    icon: string[];
}

const KpiSummaryCard = ({value, label, detail, color, icon}: KpiSummaryCardProps) => {
    return (<CCol sm={6} xl={3}>
        <CWidgetStatsA
            color={color}
            value={<>
                {value}
                <span className="fs-6 fw-normal d-block mt-1">
                    {label}
                </span>
            </>}
            title={detail}
            action={<div className="mt-2">
                <CIcon icon={icon} size="xl" className="text-white-50" />
            </div>} />
    </CCol>);
};

export default KpiSummaryCard;
