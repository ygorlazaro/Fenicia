// @ts-nocheck
import { cilCalendar, cilCheck, cilWarning } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CProgress, CRow } from "@coreui/react";
import { t } from "i18next";
import { FinancialAccountsReceivable } from "../../types/basic/dashboard/financial-accounts-receivable";
import { KPISummary } from "../../types/basic/dashboard/kpi-summary";
import formatCurrency from "../../utils/format-currency";

interface AdditionalInfoProps {
    kpi: KPISummary;
    accountsReceivable: FinancialAccountsReceivable;
}

export function AdditionalInfo({ kpi, accountsReceivable }: AdditionalInfoProps) {
    return (
        <CRow
            xs={{
                gutter: 4
            }}
        >
            <CCol md={6}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilCalendar} className="me-2" />
                        <strong>{t("dashboard.stockValue")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        <div className="text-center py-3">
                            <div className="fs-2 fw-semibold text-primary">{formatCurrency(kpi.totalStockValue)}</div>
                            <p className="text-muted mb-0">{t("dashboard.totalStockValue")}</p>
                            <div className="mt-3">
                                <CProgress value={100} color="primary" className="mb-2" />
                                <small className="text-body-secondary">
                                    {kpi.totalProducts} {t("dashboard.products")}
                                </small>
                            </div>
                        </div>
                    </CCardBody>
                </CCard>
            </CCol>

            <CCol md={6}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilWarning} className="me-2" />
                        <strong>{t("dashboard.pendingOrdersAlert")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {accountsReceivable.pendingOrdersCount > 0 ? (
                            <div className="text-center py-3">
                                <div className="fs-2 fw-semibold text-warning">{accountsReceivable.pendingOrdersCount}</div>
                                <p className="text-muted mb-0">{t("dashboard.pendingOrdersAwaitingApproval")}</p>
                                <div className="mt-3">
                                    <CProgress value={(accountsReceivable.pendingOrdersCount / (accountsReceivable.pendingOrdersCount + accountsReceivable.approvedOrdersCount)) * 100} color="warning" className="mb-2" />
                                    <small className="text-body-secondary">
                                        {formatCurrency(accountsReceivable.totalPending)} {t("dashboard.pendingValue")}
                                    </small>
                                </div>
                            </div>
                        ) : (
                            <div className="text-center py-3">
                                <CIcon icon={cilCheck} className="text-success" size="4xl" />
                                <p className="text-muted mt-2 mb-0">{t("dashboard.noPendingOrders")}</p>
                            </div>
                        )}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}
