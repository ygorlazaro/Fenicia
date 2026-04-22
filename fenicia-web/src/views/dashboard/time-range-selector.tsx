import {
    CButton,
    CButtonGroup,
    CCol,
    CRow
} from '@coreui/react';
import { useTranslation } from 'react-i18next';

interface TimeRangeSelectorProps {
    days: number;
    setDays: (days: number) => void;
}

const TimeRangeSelector = ({ days, setDays }: TimeRangeSelectorProps) => { 
    const { t } = useTranslation();

    return (
        <CRow className="mb-4">
            <CCol xs={12}>
                <div className="d-flex justify-content-between align-items-center">
                    <h4 className="mb-0">{t('dashboard.financialDashboard')}</h4>
                    <CButtonGroup>
                        <CButton
                            color={days === 30 ? 'primary' : 'outline-primary'}
                            onClick={() => setDays(30)}
                        >
                            {t('dashboard.last30Days')}
                        </CButton>
                        <CButton
                            color={days === 90 ? 'primary' : 'outline-primary'}
                            onClick={() => setDays(90)}
                        >
                            {t('dashboard.last90Days')}
                        </CButton>
                        <CButton
                            color={days === 180 ? 'primary' : 'outline-primary'}
                            onClick={() => setDays(180)}
                        >
                            {t('dashboard.last180Days')}
                        </CButton>
                    </CButtonGroup>
                </div>
            </CCol>
        </CRow>
    )
}

export default TimeRangeSelector;
