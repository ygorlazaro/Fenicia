import { cilPeople } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { CustomerOrderHistory } from "../../../../services/customer-insights-client";
import { formatCurrency } from "../../../../utils/format-currency";
import { formatDate } from "../../../../utils/format-date";

interface TopCustomersProps {
    topCustomers: CustomerOrderHistory[];
}

export function TopCustomers({ topCustomers }: TopCustomersProps) {
    return <CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilPeople} className="me-2" />
                    <strong>{t('customers.topCustomers')}</strong>
                </CCardHeader>
                <CCardBody>
                    {topCustomers.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <CTable hover responsive>
                        <CTableHead>
                            <CTableRow>
                                <CTableHeaderCell>#</CTableHeaderCell>
                                <CTableHeaderCell>{t('customers.customer')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('customers.orders')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('customers.totalSpent')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-end">{t('customers.aov')}</CTableHeaderCell>
                                <CTableHeaderCell className="text-center">{t('customers.lastOrder')}</CTableHeaderCell>
                            </CTableRow>
                        </CTableHead>
                        <CTableBody>
                            {topCustomers.map((customer, index) => <CTableRow key={customer.customerId}>
                                <CTableDataCell>{index + 1}</CTableDataCell>
                                <CTableDataCell>
                                    <Link to={`/basic/customers?id=${customer.customerId}`} className="text-decoration-none">
                                        <strong>{customer.customerName}</strong>
                                    </Link>
                                </CTableDataCell>
                                <CTableDataCell className="text-center">{customer.orderCount}</CTableDataCell>
                                <CTableDataCell className="text-end">
                                    <strong>{formatCurrency(customer.totalSpent)}</strong>
                                </CTableDataCell>
                                <CTableDataCell className="text-end">{formatCurrency(customer.averageOrderValue)}</CTableDataCell>
                                <CTableDataCell className="text-center">{formatDate(customer.lastOrderDate)}</CTableDataCell>
                            </CTableRow>)}
                        </CTableBody>
                    </CTable>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
