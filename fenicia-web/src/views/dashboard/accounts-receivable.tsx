import { cilClock } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow } from "@coreui/react";
import { CChartDoughnut } from "@coreui/react-chartjs";
import { getStyle } from "@coreui/utils";
import { useTranslation } from "react-i18next";
import { FinancialAccountsReceivable } from "../../services/financial-dashboard-client";
import { formatCurrency } from "../../utils/format-currency";

interface AccountsReceivableProps {
    accountsReceivable: FinancialAccountsReceivable
}

export function AccountsReceivable({
    accountsReceivable
}: AccountsReceivableProps) {

    const { t } = useTranslation();

    const getAccountsReceivableChartData = () => {
        return {
            labels: [t('dashboard.pending'), t('dashboard.approved')],
            datasets: [
                {
                    backgroundColor: [getStyle('--cui-warning'), getStyle('--cui-success')],
                    data: [accountsReceivable.totalPending, accountsReceivable.totalApproved],
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
                {accountsReceivable?.totalPending === 0 && accountsReceivable?.totalApproved === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <>
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
                                    {formatCurrency((accountsReceivable?.totalPending))}
                                </div>
                                <small className="text-body-secondary">
                                    {accountsReceivable?.pendingOrdersCount} {t('dashboard.pendingOrders')}
                                </small>
                            </div>
                        </CCol>
                        <CCol xs={6}>
                            <div className="text-center">
                                <div className="text-success fw-semibold">
                                    {formatCurrency((accountsReceivable?.totalApproved))}
                                </div>
                                <small className="text-body-secondary">
                                    {accountsReceivable?.approvedOrdersCount} {t('dashboard.approvedOrders')}
                                </small>
                            </div>
                        </CCol>
                    </CRow>
                </>}
            </CCardBody>
        </CCard>
    </CCol>;
}
