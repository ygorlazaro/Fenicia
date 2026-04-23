import { cilPeople } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { OrderTopCustomer } from "../../../../types/basic/order/order-top-customer";
import formatCurrency from "../../../../utils/format-currency";

interface TopCustomersProps {
    topCustomers: OrderTopCustomer[];
}

export default function TopCustomers({ topCustomers }: TopCustomersProps) {
    return (<CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilPeople} className="me-2" />
                    <strong>{t('orders.topCustomers')}</strong>
                </CCardHeader>
                <CCardBody>
                    {topCustomers.length === 0 ? (
                        <p className="text-muted text-center">{t('common.noData')}</p>
                    ) : (
                        <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>{t('orders.customer')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('orders.orders')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('orders.totalSpent')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('orders.items')}</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {topCustomers.map((customer) => (
                                    <CTableRow key={customer.customerId}>
                                        <CTableDataCell>
                                            <Link to={`/basic/customers?id=${customer.customerId}`} className="text-decoration-none">
                                                <strong>{customer.customerName}</strong>
                                            </Link>
                                        </CTableDataCell>
                                        <CTableDataCell className="text-center">{customer.orderCount}</CTableDataCell>
                                        <CTableDataCell className="text-end">
                                            <strong>{formatCurrency(customer.totalSpent)}</strong>
                                        </CTableDataCell>
                                        <CTableDataCell className="text-end">{customer.totalItems}</CTableDataCell>
                                    </CTableRow>
                                ))}
                            </CTableBody>
                        </CTable>
                    )}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>
    );
}
