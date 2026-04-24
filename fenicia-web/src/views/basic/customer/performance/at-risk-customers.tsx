import { cilArrowBottom } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { CustomerRiskAlert } from '../../../../types/basic/customer/customer-risk-alert';
import formatCurrency from "../../../../utils/format-currency";

interface AtRiskCustomersProps {
    atRiskCustomers: CustomerRiskAlert[];
}

export function AtRiskCustomers({ atRiskCustomers }: AtRiskCustomersProps) {
    return <CRow>
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilArrowBottom} className="me-2 text-danger" />
                    <strong>{t('customers.atRiskCustomers')}</strong>
                </CCardHeader>
                <CCardBody>
                    {atRiskCustomers.length === 0 ? <p className="text-muted text-center">{t('customers.noAtRiskCustomers')}</p> : <CTable hover responsive>
                        <CTableHead>
                            <CTableRow>
                                <CTableHeaderCell>{t('customers.customer')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('customers.previousOrders')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('customers.daysSinceLastOrder')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('customers.previousTotalSpent')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('customers.riskLevel')}</CTableHeaderCell>
                            </CTableRow>
                        </CTableHead>
                        <CTableBody>
                            {atRiskCustomers.map(customer => <CTableRow key={customer.customerId}>
                                <CTableDataCell>
                                    <Link to={`/basic/customers?id=${customer.customerId}`} className="text-decoration-none">
                                        <strong>{customer.customerName}</strong>
                                    </Link>
                                </CTableDataCell>
                                <CTableDataCell className="text-center">{customer.previousOrderCount}</CTableDataCell>
                                <CTableDataCell className="text-center">
                                    <CBadge color={customer.daysSinceLastOrder >= 120 ? 'danger' : customer.daysSinceLastOrder >= 90 ? 'warning' : 'info'}>
                                        {customer.daysSinceLastOrder} {t('customers.days')}
                                    </CBadge>
                                </CTableDataCell>
                                <CTableDataCell className="text-end">{formatCurrency(customer.previousTotalSpent)}</CTableDataCell>
                                <CTableDataCell className="text-center">
                                    <CBadge color={customer.riskLevel === 'High' ? 'danger' : customer.riskLevel === 'Medium' ? 'warning' : 'info'}>
                                        {t(`customers.${customer.riskLevel.toLowerCase()}`)}
                                    </CBadge>
                                </CTableDataCell>
                            </CTableRow>)}
                        </CTableBody>
                    </CTable>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
