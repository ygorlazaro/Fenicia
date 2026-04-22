import { cilPeople } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { EmployeeOrderCount } from "../../../../services/employee-performance-client";
import { formatCurrency } from "../../../../utils/format-currency";

interface OrdersByEmployeeProps {
    ordersByEmployee: EmployeeOrderCount[];
}

export function OrdersByEmployee({
  ordersByEmployee
}: OrdersByEmployeeProps) {
  return <CRow>
            <CCol xs={12}>
                <CCard>
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilPeople} className="me-2" />
                        <strong>{t('employees.ordersByEmployee')}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {ordersByEmployee.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>{t('employees.employee')}</CTableHeaderCell>
                                    <CTableHeaderCell>{t('employees.position')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('employees.orders')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-end">{t('employees.totalValue')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('employees.firstOrder')}</CTableHeaderCell>
                                    <CTableHeaderCell className="text-center">{t('employees.lastOrder')}</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {ordersByEmployee.map(employee => <CTableRow key={employee.employeeId}>
                                    <CTableDataCell>
                                        <Link to={`/basic/employees?id=${employee.employeeId}`} className="text-decoration-none">
                                            {employee.employeeName}
                                        </Link>
                                    </CTableDataCell>
                                    <CTableDataCell>{employee.positionName}</CTableDataCell>
                                    <CTableDataCell className="text-center">
                                        <CBadge color="info">{employee.orderCount}</CBadge>
                                    </CTableDataCell>
                                    <CTableDataCell className="text-end">{formatCurrency(employee.totalValue)}</CTableDataCell>
                                    <CTableDataCell className="text-center">{new Date(employee.firstOrderDate).toLocaleDateString()}</CTableDataCell>
                                    <CTableDataCell className="text-center">{new Date(employee.lastOrderDate).toLocaleDateString()}</CTableDataCell>
                                </CTableRow>)}
                            </CTableBody>
                        </CTable>}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>;
}
  