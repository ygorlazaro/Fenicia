import { cilClock } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { CChartDoughnut } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { useTranslation } from "react-i18next";
import { FinancialAccountsReceivable } from '../../types/basic/dashboard/financial-accounts-receivable';
import formatCurrency from "../../utils/format-currency";

interface AccountsReceivableProps {
    data: FinancialAccountsReceivable
}

const AccountsReceivable = ({
    data
}: AccountsReceivableProps) => {
    const { t } = useTranslation();

    const getAccountsReceivableChartData = () => {
        return {
            labels: [t('dashboard.pending'), t('dashboard.approved')],
            datasets: [
                {
                    backgroundColor: [getStyle('--cui-warning'), getStyle('--cui-success')],
                    data: [data.totalPending, data.totalApproved],
                },
            ],
        };
    };

    return <CCol md={4}>
        <CCard className="mb-4">
            <CCardHeader className="d-flex align-items-center">
                <CIcon icon={cilClock} className="me-2" />
                <strong>{t('dashboard.accountsReceivable')}</strong>
            </CCardHeader>
            <CCardBody>
                {data?.totalPending === 0 && data?.totalApproved === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <>
                    <CChartDoughnut data={getAccountsReceivableChartData()} options={{
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {
                            legend: {
                                position: 'bottom'
                            }
                        }
                    }} />
                    <CRow className="mt-3" xs={{
                        gutter: 2
                    }}>
                        <CCol xs={6}>
                            <div className="text-center">
                                <div className="text-warning fw-semibold">
                                    {formatCurrency((data?.totalPending))}
                                </div>
                                <small className="text-body-secondary">
                                    {data?.pendingOrdersCount} {t('dashboard.pendingOrders')}
                                </small>
                            </div>
                        </CCol>
                        <CCol xs={6}>
                            <div className="text-center">
                                <div className="text-success fw-semibold">
                                    {formatCurrency((data?.totalApproved))}
                                </div>
                                <small className="text-body-secondary">
                                    {data?.approvedOrdersCount} {t('dashboard.approvedOrders')}
                                </small>
                            </div>
                        </CCol>
                    </CRow>
                </>}
            </CCardBody>
        </CCard>
    </CCol>;
}

export default AccountsReceivable;
